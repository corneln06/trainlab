import type { HeartRateZone } from './useZones'

const zoneColors: Record<string, string> = {
  Z1: 'bg-base-300',
  Z2: 'bg-info/30',
  Z3: 'bg-success/30',
  Z4: 'bg-warning/30',
  Z5a: 'bg-error/20',
  Z5b: 'bg-error/35',
  Z5c: 'bg-error/50',
}

function range(zone: HeartRateZone): string {
  if (zone.minBpm === null) return `< ${(zone.maxBpm ?? 0) + 1}`
  if (zone.maxBpm === null) return `≥ ${zone.minBpm}`
  return `${zone.minBpm} – ${zone.maxBpm}`
}

export default function ZoneTable({ zones }: { zones: HeartRateZone[] }) {
  return (
    <table className="table table-sm">
      <thead>
        <tr>
          <th>Zone</th>
          <th>Heart rate (bpm)</th>
        </tr>
      </thead>
      <tbody>
        {zones.map((zone) => (
          <tr key={zone.name} className={zoneColors[zone.name] ?? ''}>
            <td className="font-semibold">{zone.name}</td>
            <td>{range(zone)}</td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}