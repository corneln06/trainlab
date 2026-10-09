-- R-15 spike: TimescaleDB hypertable + PostGIS
-- Run with:
--   docker exec -i trainlab-db psql -U trainlab -d trainlab < spikes/timescale/spike.sql

\timing on

-- Start clean so the script can be run again
DROP TABLE IF EXISTS spike_records;

-- 1. Extensions
CREATE EXTENSION IF NOT EXISTS timescaledb;
CREATE EXTENSION IF NOT EXISTS postgis;

-- 2. Table + hypertable
CREATE TABLE spike_records (
                               activity_id integer,
                               time        timestamptz,
                               hr          smallint,
                               lat         double precision,
                               lon         double precision,
                               PRIMARY KEY (activity_id, time)
);

SELECT create_hypertable('spike_records', by_range('time'));

-- 3. 100,000 fake records: 10 activities x 10,000 seconds
INSERT INTO spike_records (activity_id, time, hr, lat, lon)
SELECT
    (n - 1) / 10000,
    timestamptz '2026-09-23 10:00:00+00' + n * interval '1 second',
    (120 + random() * 50)::smallint,
    55.86 + n * 0.00001 + sin(n / 50.0) * 0.0005,
    9.85  + n * 0.00001
FROM generate_series(1, 100000) AS n;

ANALYZE spike_records;

-- 4. Time-range query (10 minutes of activity 3)
EXPLAIN ANALYZE
SELECT count(*), avg(hr)
FROM spike_records
WHERE activity_id = 3
  AND time BETWEEN '2026-09-23 18:30:00+00' AND '2026-09-23 18:40:00+00';

-- 5. PostGIS: build a line for activity 0 and simplify it
SELECT
    ST_NPoints(line)                                         AS original_points,
    ST_NPoints(ST_SimplifyPreserveTopology(line, 0.00005))   AS simplified_points
FROM (
         SELECT ST_MakeLine(ST_SetSRID(ST_MakePoint(lon, lat), 4326) ORDER BY time) AS line
         FROM spike_records
         WHERE activity_id = 0
     ) AS track;