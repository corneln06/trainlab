import type { Sport, ZoneSet } from './useZones'

export function todayIso(): string {
  const now = new Date()
  const pad = (n: number) => String(n).padStart(2, '0')
  return `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())}`
}

export function currentZoneSet(sets: ZoneSet[], sport: Sport, today: string): ZoneSet | undefined {
  return sets
    .filter((set) => set.sport === sport && set.effectiveFrom <= today)
    .sort((a, b) => b.effectiveFrom.localeCompare(a.effectiveFrom))[0]
}