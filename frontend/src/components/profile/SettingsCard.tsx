import { useState, type FormEvent } from 'react'
import { getErrorMessage } from '../../api/errors'
import { updateSettings } from '../../api/profile'
import type { Settings, TwoFactorMethod } from '../../api/types'
import { Alert, Button, RadioGroup, SelectField, SwitchField } from '../Form'

const reminderOptions = [
  { value: '15', label: '15 minutter før' },
  { value: '30', label: '30 minutter før' },
  { value: '60', label: '1 time før' },
  { value: '120', label: '2 timer før' },
  { value: '1440', label: '1 dag før' },
]

interface SettingsCardProps {
  settings: Settings
  onSaved: (settings: Settings) => void
}

/** Påmindelser (kanal og tidspunkt) og hvordan loginkoden (2FA) sendes. */
export default function SettingsCard({ settings, onSaved }: SettingsCardProps) {
  // SMS kan kun bruges med et bekræftet nummer. Er nummeret ikke bekræftet, vises og gemmes SMS som slået fra –
  // ellers kunne brugeren sidde fast med en tændt kontakt, de ikke kan slukke.
  const smsAvailable = settings.phoneNumberConfirmed

  const [emailEnabled, setEmailEnabled] = useState(settings.emailEnabled)
  const [smsEnabled, setSmsEnabled] = useState(settings.smsEnabled && smsAvailable)
  const [minutesBefore, setMinutesBefore] = useState(String(settings.reminderMinutesBefore))
  const [twoFactorMethod, setTwoFactorMethod] = useState<TwoFactorMethod>(
    settings.twoFactorMethod === 'Sms' && !smsAvailable ? 'Email' : settings.twoFactorMethod,
  )
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [saved, setSaved] = useState(false)

  const smsOn = smsEnabled && smsAvailable
  const remindersOff = !emailEnabled && !smsOn

  // Vis også en værdi der ikke er i listen (f.eks. sat via API'et)
  const options = reminderOptions.some((o) => o.value === minutesBefore)
    ? reminderOptions
    : [...reminderOptions, { value: minutesBefore, label: `${minutesBefore} minutter før` }]

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setSaved(false)
    setSaving(true)

    try {
      const result = await updateSettings({
        emailEnabled,
        smsEnabled: smsOn,
        reminderMinutesBefore: Number(minutesBefore),
        timeZone: settings.timeZone,
        twoFactorMethod,
      })
      onSaved(result)
      setSaved(true)
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setSaving(false)
    }
  }

  return (
    <form className="card h-100" onSubmit={handleSubmit}>
      <div className="card-body">
        {error && <Alert>{error}</Alert>}
        {saved && <Alert kind="success">Indstillingerne er gemt.</Alert>}

        <h2 className="h6 card-title mb-3">Påmindelser</h2>
        <SwitchField
          label="Påmindelser på e-mail"
          checked={emailEnabled}
          onChange={(checked) => {
            setEmailEnabled(checked)
            setSaved(false)
          }}
        />
        <SwitchField
          label="Påmindelser på SMS"
          checked={smsOn}
          disabled={!smsAvailable}
          hint={smsAvailable ? undefined : 'Bekræft dit telefonnummer for at bruge SMS.'}
          onChange={(checked) => {
            setSmsEnabled(checked)
            setSaved(false)
          }}
        />

        <div className="mt-3">
          <SelectField
            label="Send påmindelsen"
            value={minutesBefore}
            options={options}
            disabled={remindersOff}
            onChange={(e) => {
              setMinutesBefore(e.target.value)
              setSaved(false)
            }}
          />
        </div>
        {remindersOff && <p className="small text-body-secondary mt-n2">Påmindelser er slået fra.</p>}

        <hr />

        <h2 className="h6 card-title mb-3">Totrinsbekræftelse (2FA)</h2>
        <RadioGroup
          legend="Send loginkoden via"
          name="twoFactorMethod"
          value={twoFactorMethod}
          options={[
            { value: 'Email', label: 'E-mail', disabled: !settings.emailConfirmed },
            { value: 'Sms', label: 'SMS', disabled: !smsAvailable },
          ]}
          hint="Hver gang du logger ind, sender vi en engangskode, som du skal indtaste efter din adgangskode."
          onChange={(value) => {
            setTwoFactorMethod(value)
            setSaved(false)
          }}
        />
      </div>

      <div className="card-footer bg-transparent text-end">
        <Button type="submit" loading={saving}>
          Gem indstillinger
        </Button>
      </div>
    </form>
  )
}
