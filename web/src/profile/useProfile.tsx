import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from '../api/client'
import type { components } from '../api/schema'

export type Profile = components['schemas']['AthleteProfileResponse']
export type ProfileUpdate = components['schemas']['UpdateAthleteProfileRequest']

export const profileKey = ['profile']

export function useProfile() {
  return useQuery({
    queryKey: profileKey,
    queryFn: async () => {
      const { data, error } = await api.GET('/api/profile')
      if (error || !data) throw new Error('Could not load your profile.')
      return data
    },
  })
}
export class ValidationError extends Error {
  errors: Record<string, string[]>

  constructor(errors: Record<string, string[]>) {
    super('Please fix the highlighted fields.')
    this.errors = errors
  }
}
export function useUpdateProfile() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: async (update: ProfileUpdate) => {
      const { data, error, response } = await api.PUT('/api/profile', { body: update })
      if (response.status === 400) throw new ValidationError(error?.errors ?? {})
      if (error || !data) throw new Error('Could not save your profile.')
      return data
    },
    onSuccess: (saved) => {
      queryClient.setQueryData(profileKey, saved)
    },
  })
}