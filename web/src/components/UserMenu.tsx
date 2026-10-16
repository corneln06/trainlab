import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router'
import { logout } from '../api/auth'
import { useCurrentUser } from '../auth/useCurrentUser'

export default function UserMenu() {
  const { data: user } = useCurrentUser()
  const queryClient = useQueryClient()
  const navigate = useNavigate()

  const mutation = useMutation({
    mutationFn: logout,
    onSuccess: () => {
      queryClient.clear()
      navigate('/login', { replace: true })
    },
  })

  if (!user) return null
  return (
    <div className="flex items-center gap-2">
      <span className="hidden text-sm opacity-70 sm:inline">{user.email}</span>
      <button className="btn btn-ghost btn-sm" onClick={() => mutation.mutate()} disabled={mutation.isPending}>
        Log out
      </button>
    </div>
  )
}