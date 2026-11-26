// Typer der matcher backendens DTO'er (backend/LevelUp.Api/Dtos)

export type TwoFactorMethod = 'Email' | 'Sms'

export interface AuthUser {
  id: number
  email: string
  displayName: string
}

export interface RegisterRequest {
  email: string
  password: string
  displayName: string
  phoneNumber?: string
  confirmationMethod: TwoFactorMethod
}

export interface RegisterResponse {
  message: string
  confirmationMethod: TwoFactorMethod
  sentTo: string
}

export interface ConfirmAccountRequest {
  email: string
  code: string
  method: TwoFactorMethod
}

export interface LoginRequest {
  email: string
  password: string
}

/** Svar på login trin 1 - koden er sendt. */
export interface LoginResponse {
  twoFactorToken: string
  method: TwoFactorMethod
  sentTo: string
}

/** Svar på login trin 2 - brugeren er logget ind. */
export interface AuthResponse {
  accessToken: string
  expiresAt: string
  user: AuthUser
}

export interface MessageResponse {
  message: string
}

export interface Profile {
  id: number
  email: string
  displayName: string
  level: number
  totalXp: number
  xpIntoLevel: number
  xpForNextLevel: number
  coinBalance: number
  completedTasks: number
  inventoryCount: number
  createdAt: string
}

export interface Settings {
  /** Påmindelser på e-mail. */
  emailEnabled: boolean
  /** Påmindelser på SMS - kræver bekræftet telefonnummer. */
  smsEnabled: boolean
  /** Minutter før en opgave påmindelsen sendes (5-1440). */
  reminderMinutesBefore: number
  timeZone: string
  /** Hvordan loginkoden (2FA) sendes. */
  twoFactorMethod: TwoFactorMethod
  emailConfirmed: boolean
  phoneNumber: string | null
  phoneNumberConfirmed: boolean
}

export type UpdateSettingsRequest = Pick<
  Settings,
  'emailEnabled' | 'smsEnabled' | 'reminderMinutesBefore' | 'timeZone' | 'twoFactorMethod'
>

export type TaskStatus = 'Pending' | 'Completed'

/** Tidspunkter er lokale uden tidszone, fx. "2026-11-25T09:00:00". */
export interface TaskItem {
  id: number
  title: string
  description: string | null
  scheduledStart: string
  scheduledEnd: string | null
  status: TaskStatus
  completedAt: string | null
  createdAt: string
  /** Fælles id for opgaver i en gentagende serie, ellers null. */
  seriesId: string | null
}

export interface SaveTaskRequest {
  title: string
  description: string | null
  scheduledStart: string
  scheduledEnd: string | null
}

export type RecurrenceFrequency = 'Daily' | 'Weekdays' | 'Weekly' | 'Biweekly' | 'Monthly'

export interface CreateTaskRequest extends SaveTaskRequest {
  /** Udelades for en opgave der kun sker en gang. */
  repeat?: RecurrenceFrequency
  /** Sidste dato i serien, fx. "2026-12-31". Krævet sammen med repeat. */
  repeatUntil?: string
}

/** XpGained/pointsGained er for selve opgaven. Bonus for en gennemført udfordring står i challenge. */
export interface CompleteTaskResponse {
  task: TaskItem
  xpGained: number
  pointsGained: number
  totalXp: number
  coinBalance: number
  level: number
  leveledUp: boolean
  /** Sat hvis opgaven hører til en udfordring. */
  challenge: ChallengeProgress | null
  unlockedAchievements: Achievement[]
}

export type AchievementType = 'Level' | 'TasksCompleted' | 'ItemsBought' | 'ChallengesCompleted'

export interface Achievement {
  id: number
  name: string
  description: string | null
  type: AchievementType
  requiredCount: number
  /** Brugerens tal, højst requiredCount. */
  currentCount: number
  /** Null hvis det ikke er låst op endnu. */
  unlockedAt: string | null
}

export type ChallengeStatus = 'NotStarted' | 'Active' | 'Completed'

export interface Challenge {
  id: number
  name: string
  description: string | null
  taskTitle: string
  repeat: RecurrenceFrequency
  requiredCount: number
  rewardXp: number
  rewardCoins: number
  endDate: string | null
  status: ChallengeStatus
  progress: number
  startedAt: string | null
  completedAt: string | null
}

export interface ChallengeProgress {
  challengeId: number
  name: string
  progress: number
  requiredCount: number
  completed: boolean
  rewardXp: number
  rewardCoins: number
}

export interface ShopItem {
  id: number
  name: string
  description: string | null
  price: number
  /** "Hat", "Outfit" eller "Accessory". */
  category: string
  /** true hvis brugeren allerede har købt genstanden. */
  owned: boolean
}

export interface InventoryItem {
  id: number
  shopItemId: number
  name: string
  description: string | null
  category: string
  isEquipped: boolean
  purchasedAt: string
}

export interface PurchaseResponse {
  item: InventoryItem
  coinBalance: number
  unlockedAchievements: Achievement[]
}

/** Fejlsvar fra API'et (ProblemDetails). */
export interface ProblemDetails {
  title?: string
  status?: number
  detail?: string
  code?: string
  errors?: Record<string, string[]>
}
