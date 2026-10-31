import ProfileForm from '../profile/ProfileForm'
import { useProfile } from '../profile/useProfile'

export default function SettingsPage() {
  const { data, isPending, isError } = useProfile()

  return (
    <div className="max-w-xl">
      <h1 className="text-2xl font-bold">Settings</h1>
      <p className="mt-1 opacity-70">These values are used to calculate your training load.</p>

      <div className="mt-6">
        {isPending && <span className="loading loading-spinner" />}
        {isError && (
          <div role="alert" className="alert alert-error">Could not load your profile.</div>
        )}
        {data && <ProfileForm profile={data} />}
      </div>
    </div>
  )
}