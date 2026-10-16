import { useState, type FormEvent } from 'react'
import { Link, useLocation, useNavigate } from 'react-router'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { login } from '../api/auth'
import { currentUserKey } from '../auth/useCurrentUser'

export default function LoginPage() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const navigate = useNavigate()
  const location = useLocation()
  const queryClient = useQueryClient()

  const from = (location.state as { from?: string } | null)?.from ?? '/'

  const mutation = useMutation({
    mutationFn: login,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: currentUserKey })
      navigate(from, { replace: true })
    },
  })

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    mutation.mutate({ email, password })
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-base-200 p-4">
      <form onSubmit={handleSubmit} className="card w-full max-w-sm bg-base-100 shadow">
        <div className="card-body gap-4">
          <h1 className="card-title text-2xl">Log in to TrainLab</h1>

          <label className="flex flex-col gap-1">
            <span className="text-sm">Email</span>
            <input
              type="email"
              className="input w-full"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              autoComplete="email"
              required
            />
          </label>

          <label className="flex flex-col gap-1">
            <span className="text-sm">Password</span>
            <input
              type="password"
              className="input w-full"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              autoComplete="current-password"
              required
            />
          </label>

          {mutation.isError && (
            <div role="alert" className="alert alert-error">
              {mutation.error.message}
            </div>
          )}

          <button type="submit" className="btn btn-primary" disabled={mutation.isPending}>
            {mutation.isPending ? 'Logging in…' : 'Log in'}
          </button>

          <p className="text-center text-sm">
            No account yet? <Link to="/register" className="link">Register</Link>
          </p>
        </div>
      </form>
    </div>
  )
}