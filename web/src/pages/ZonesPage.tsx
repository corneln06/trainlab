import NewZoneSetForm from '../zones/NewZoneSetForm'
import ZoneHistory from '../zones/ZoneHistory'
import ZoneTable from '../zones/ZoneTable'
import { currentZoneSet, todayIso } from '../zones/currentZoneSet'
import { useZoneSets, type Sport } from '../zones/useZones'

const sports: Sport[] = ['Running', 'Cycling']

export default function ZonesPage() {
  const { data: sets, isPending, isError } = useZoneSets()
  const today = todayIso()

  return (
    <div className="flex max-w-4xl flex-col gap-6">
      <div>
        <h1 className="text-2xl font-bold">Heart-rate zones</h1>
        <p className="mt-1 opacity-70">Based on your lactate threshold, per sport.</p>
      </div>

      {isPending && <span className="loading loading-spinner" />}
      {isError && <div role="alert" className="alert alert-error">Could not load your zones.</div>}

      {sets && (
        <div className="grid gap-6 md:grid-cols-2">
          {sports.map((sport) => {
            const setsForSport = sets.filter((s) => s.sport === sport)
            const current = currentZoneSet(sets, sport, today)

            return (
              <div key={sport} className="card bg-base-100 shadow">
                <div className="card-body gap-4">
                  <h2 className="card-title">
                    {sport}
                    {current && <span className="badge badge-outline">LTHR {current.lthr}</span>}
                  </h2>

                  {current ? (
                    <ZoneTable zones={current.zones} />
                  ) : (
                    <p className="text-sm opacity-70">No {sport.toLowerCase()} zones yet. Add them below.</p>
                  )}

                  {setsForSport.length > 0 && (
                    <div>
                      <p className="mb-1 text-sm font-semibold">History</p>
                      <ZoneHistory sets={setsForSport} currentId={current?.id} />
                    </div>
                  )}
                </div>
              </div>
            )
          })}
        </div>
      )}

      <NewZoneSetForm />
    </div>
  )
}