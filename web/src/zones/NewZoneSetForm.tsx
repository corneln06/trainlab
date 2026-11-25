import { useState, type FormEvent } from 'react'
import { ValidationError } from '../profile/useProfile'
import ZoneTable from './ZoneTable'
import { todayIso } from './currentZoneSet'
import { ConflictError, isValidLthr, useCreateZoneSet, useZonePreview, type Sport } from './useZones'

export default function NewZoneSetForm() {
  const [sport, setSport] = useState<Sport>('Running')
  const [lthr, setLthr] = useState('')
  const [effectiveFrom, setEffectiveFrom] = useState(todayIso)

  const lthrNumber = lthr === '' ? NaN : Number(lthr)
  const preview = useZonePreview(sport, lthrNumber)
  const mutation = useCreateZoneSet()

  const fieldErrors = mutation.error instanceof ValidationError ? mutation.error.errors : {}
  const errorsFor = (field: string) =>
    fieldErrors[field] ?? fieldErrors[field[0].toLowerCase() + field.slice(1)] ?? []

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    mutation.mutate(
      { sport, lthr: lthrNumber, effectiveFrom },
      { onSuccess: () => setLthr('') },
    )
  }

  return (
    <form onSubmit={handleSubmit} className="card bg-base-100 shadow">
      <div className="card-body gap-4">
        <h2 className="card-title">New zones</h2>
        <p className="text-sm opacity-70">
          Your zones are calculated from your lactate threshold heart rate (LTHR).
          Older activities keep the zones that applied at their date.
        </p>

        <div className="grid gap-4 sm:grid-cols-3">
          <label className="flex flex-col gap-1">
            <span className="text-sm">Sport</span>
            <select className="select w-full" value={sport} onChange={(e) => setSport(e.target.value as Sport)}>
              <option value="Running">Running</option>
              <option value="Cycling">Cycling</option>
            </select>
          </label>

          <label className="flex flex-col gap-1">
            <span className="text-sm">LTHR (bpm)</span>
            <input
              type="number"
              className="input w-full"
              value={lthr}
              onChange={(e) => setLthr(e.target.value)}
              placeholder="e.g. 181"
              required
            />
            {errorsFor('Lthr').length > 0 && <span className="text-sm text-error">{errorsFor('Lthr').join(' ')}</span>}
          </label>

          <label className="flex flex-col gap-1">
            <span className="text-sm">Valid from</span>
            <input
              type="date"
              className="input w-full"
              value={effectiveFrom}
              max={todayIso()}
              onChange={(e) => setEffectiveFrom(e.target.value)}
              required
            />
            {errorsFor('EffectiveFrom').length > 0 && (
              <span className="text-sm text-error">{errorsFor('EffectiveFrom').join(' ')}</span>
            )}
          </label>
        </div>

        {isValidLthr(lthrNumber) && preview.data && (
          <div>
            <p className="mb-1 text-sm font-semibold">Preview</p>
            <ZoneTable zones={preview.data} />
          </div>
        )}
        {lthr !== '' && !isValidLthr(lthrNumber) && (
          <p className="text-sm opacity-70">Enter an LTHR between 100 and 220 to see your zones.</p>
        )}

        {mutation.error instanceof ConflictError && (
          <div role="alert" className="alert alert-warning">{mutation.error.message}</div>
        )}
        {mutation.isError && !(mutation.error instanceof ConflictError) && !(mutation.error instanceof ValidationError) && (
          <div role="alert" className="alert alert-error">{mutation.error.message}</div>
        )}
        {mutation.isSuccess && <div role="status" className="alert alert-success">Zones saved.</div>}

        <button type="submit" className="btn btn-primary self-start" disabled={mutation.isPending || !isValidLthr(lthrNumber)}>
          {mutation.isPending ? 'Saving…' : 'Save zones'}
        </button>
      </div>
    </form>
  )
}