import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { login, register, type Credentials } from '../api/auth'
import { currentUserKey } from '../auth/useCurrentUser'

export default function RegisterPage() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const navigate = useNavigate()
  const queryClient = useQueryClient()

  const mutation = useMutation({
    mutationFn: async (credentials: Credentials) => {
      await register(credentials)
      await login(credentials)
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: currentUserKey })
      navigate('/', { replace: true })
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
          <h1 className="card-title text-2xl">Create your account</h1>

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
              autoComplete="new-password"
              required
            />
          </label>

          {mutation.isError && (
            <div role="alert" className="alert alert-error">
              {mutation.error.message}
            </div>
          )}

          <button type="submit" className="btn btn-primary" disabled={mutation.isPending}>
            {mutation.isPending ? 'Creating account…' : 'Register'}
          </button>

          <p className="text-center text-sm">
            Already have an account? <Link to="/login" className="link">Log in</Link>
          </p>
        </div>
      </form>
    </div>
  )
}