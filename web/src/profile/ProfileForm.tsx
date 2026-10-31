import { useState, type FormEvent, type ReactNode } from 'react'
import { useUpdateProfile, ValidationError, type Profile, type ProfileUpdate } from './useProfile'

type FormState = {
  sex: '' | 'Male' | 'Female'
  birthYear: string
  weightKg: string
  restingHr: string
  maxHr: string
  timeZone: string
}

const toText = (value: number | null) => (value === null ? '' : String(value))
const toNumberOrNull = (value: string) => (value.trim() === '' ? null : Number(value))

function fromProfile(profile: Profile): FormState {
  return {
    sex: profile.sex ?? '',
    birthYear: toText(profile.birthYear),
    weightKg: toText(profile.weightKg),
    restingHr: toText(profile.restingHr),
    maxHr: toText(profile.maxHr),
    timeZone: profile.timeZone,
  }
}

const allTimeZones = Intl.supportedValuesOf('timeZone')

function Field({ label, errors, children }: { label: string; errors: string[]; children: ReactNode }) {
  return (
    <label className="flex flex-col gap-1">
      <span className="text-sm">{label}</span>
      {children}
      {errors.length > 0 && <span className="text-sm text-error">{errors.join(' ')}</span>}
    </label>
  )
}

export default function ProfileForm({ profile }: { profile: Profile }) {
  const [form, setForm] = useState<FormState>(() => fromProfile(profile))
  const mutation = useUpdateProfile()

  const fieldErrors = mutation.error instanceof ValidationError ? mutation.error.errors : {}
  const errorsFor = (field: string) =>
    fieldErrors[field] ?? fieldErrors[field[0].toLowerCase() + field.slice(1)] ?? []

  const timeZones = allTimeZones.includes(form.timeZone) ? allTimeZones : [form.timeZone, ...allTimeZones]

  function update<K extends keyof FormState>(field: K, value: FormState[K]) {
    setForm((current) => ({ ...current, [field]: value }))
    if (mutation.isSuccess) mutation.reset()
  }

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    if (form.sex === '') return

    const request: ProfileUpdate = {
      sex: form.sex,
      birthYear: toNumberOrNull(form.birthYear),
      weightKg: toNumberOrNull(form.weightKg),
      restingHr: toNumberOrNull(form.restingHr),
      maxHr: toNumberOrNull(form.maxHr),
      timeZone: form.timeZone,
    }
    mutation.mutate(request)
  }

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4">
      <Field label="Sex" errors={errorsFor('Sex')}>
        <select
          className="select w-full"
          value={form.sex}
          onChange={(e) => update('sex', e.target.value as FormState['sex'])}
          required
        >
          <option value="" disabled>Choose…</option>
          <option value="Male">Male</option>
          <option value="Female">Female</option>
        </select>
      </Field>

      <Field label="Birth year" errors={errorsFor('BirthYear')}>
        <input
          type="number"
          className="input w-full"
          value={form.birthYear}
          onChange={(e) => update('birthYear', e.target.value)}
        />
      </Field>

      <Field label="Weight (kg)" errors={errorsFor('WeightKg')}>
        <input
          type="number"
          step="0.1"
          className="input w-full"
          value={form.weightKg}
          onChange={(e) => update('weightKg', e.target.value)}
        />
      </Field>

      <div className="grid gap-4 sm:grid-cols-2">
        <Field label="Resting heart rate (bpm)" errors={errorsFor('RestingHr')}>
          <input
            type="number"
            className="input w-full"
            value={form.restingHr}
            onChange={(e) => update('restingHr', e.target.value)}
          />
        </Field>

        <Field label="Max heart rate (bpm)" errors={errorsFor('MaxHr')}>
          <input
            type="number"
            className="input w-full"
            value={form.maxHr}
            onChange={(e) => update('maxHr', e.target.value)}
          />
        </Field>
      </div>

      <Field label="Time zone" errors={errorsFor('TimeZone')}>
        <select
          className="select w-full"
          value={form.timeZone}
          onChange={(e) => update('timeZone', e.target.value)}
        >
          {timeZones.map((zone) => (
            <option key={zone} value={zone}>{zone}</option>
          ))}
        </select>
      </Field>

      {mutation.isError && !(mutation.error instanceof ValidationError) && (
        <div role="alert" className="alert alert-error">{mutation.error.message}</div>
      )}
      {mutation.error instanceof ValidationError && (
        <div role="alert" className="alert alert-warning">{mutation.error.message}</div>
      )}
      {mutation.isSuccess && (
        <div role="status" className="alert alert-success">Saved.</div>
      )}

      <button type="submit" className="btn btn-primary self-start" disabled={mutation.isPending}>
        {mutation.isPending ? 'Saving…' : 'Save'}
      </button>
    </form>
  )
}