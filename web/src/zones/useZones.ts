import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from '../api/client'
import type { components } from '../api/schema'
import { ValidationError } from '../profile/useProfile'

export type ZoneSet = components['schemas']['ZoneSetResponse']
export type HeartRateZone = components['schemas']['HeartRateZone']
export type CreateZoneSet = components['schemas']['CreateZoneSetRequest']
export type Sport = ZoneSet['sport']

export const zonesKey = ['zones']

export class ConflictError extends Error {}

export function isValidLthr(lthr: number): boolean {
  return Number.isInteger(lthr) && lthr >= 100 && lthr <= 220
}

export function useZoneSets() {
  return useQuery({
    queryKey: zonesKey,
    queryFn: async () => {
      const { data, error } = await api.GET('/api/zones')
      if (error || !data) throw new Error('Could not load your zones.')
      return data
    },
  })
}

export function useZonePreview(sport: Sport, lthr: number) {
  return useQuery({
    queryKey: ['zones', 'preview', sport, lthr],
    queryFn: async () => {
      const { data, error } = await api.GET('/api/zones/preview', {
        params: { query: { sport, lthr } },
      })
      if (error || !data) throw new Error('Could not calculate zones.')
      return data
    },
    enabled: isValidLthr(lthr),         
    placeholderData: keepPreviousData,  
  })
}

export function useCreateZoneSet() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (request: CreateZoneSet) => {
      const { data, error, response } = await api.POST('/api/zones', { body: request })
      if (response.status === 409) {
        throw new ConflictError(`You already have ${request.sport.toLowerCase()} zones starting on that date.`)
      }
      if (response.status === 400) {
        const problem = error as { errors?: Record<string, string[]> } | undefined
        throw new ValidationError(problem?.errors ?? {})
      }
      if (!data) throw new Error('Could not save your zones.')
      return data
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: zonesKey }),
  })
}