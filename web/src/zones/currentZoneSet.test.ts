import { describe, expect, it } from 'vitest'
import { currentZoneSet } from './currentZoneSet'
import type { ZoneSet } from './useZones'

const set = (lthr: number, effectiveFrom: string, sport: ZoneSet['sport'] = 'Running'): ZoneSet => ({
  id: crypto.randomUUID(),
  sport,
  lthr,
  effectiveFrom,
  zones: [],
})

const sets = [set(176, '2026-01-01'), set(181, '2026-09-15')]

describe('currentZoneSet', () => {
  it.each([
    ['2025-12-31', undefined],
    ['2026-01-01', 176],
    ['2026-03-01', 176],
    ['2026-09-15', 181],
    ['2026-11-25', 181],
  ])('on %s returns LTHR %s', (today, expectedLthr) => {
    expect(currentZoneSet(sets, 'Running', today)?.lthr).toBe(expectedLthr)
  })

  it('ignores other sports', () => {
    expect(currentZoneSet(sets, 'Cycling', '2026-11-25')).toBeUndefined()
  })
})