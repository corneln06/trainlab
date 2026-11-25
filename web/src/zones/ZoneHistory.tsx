import type { ZoneSet } from './useZones'

export default function ZoneHistory({ sets, currentId }: { sets: ZoneSet[]; currentId?: string }) {
  const newestFirst = [...sets].sort((a, b) => b.effectiveFrom.localeCompare(a.effectiveFrom))

  return (
    <ul className="flex flex-col gap-1 text-sm">
      {newestFirst.map((set) => (
        <li key={set.id} className="flex items-center gap-2">
          <span className="w-24 opacity-70">{set.effectiveFrom}</span>
          <span>LTHR {set.lthr}</span>
          {set.id === currentId && <span className="badge badge-primary badge-sm">current</span>}
          {set.effectiveFrom > (sets.find((s) => s.id === currentId)?.effectiveFrom ?? '') &&
            set.id !== currentId && <span className="badge badge-ghost badge-sm">upcoming</span>}
        </li>
      ))}
    </ul>
  )
}