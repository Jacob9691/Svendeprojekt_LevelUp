import type { Achievement, AchievementType, CompleteTaskResponse, RecurrenceFrequency } from '../api/types'

/** Beskeden der vises, når en opgave er gennemført – inkl. udfordring, level og nye achievements. */
export function rewardMessage(result: CompleteTaskResponse): string {
  const parts = [`Godt gået! +${result.xpGained} XP og +${result.pointsGained} points.`]

  if (result.challenge) {
    const { name, progress, requiredCount, completed, rewardXp, rewardCoins } = result.challenge
    parts.push(
      completed
        ? `🏁 Udfordringen "${name}" er gennemført! Bonus: +${rewardXp} XP og +${rewardCoins} points.`
        : `Udfordring "${name}": ${progress}/${requiredCount}.`,
    )
  }

  if (result.leveledUp) {
    parts.push(`Du er steget til level ${result.level}!`)
  }

  const achievements = achievementsMessage(result.unlockedAchievements)
  if (achievements) {
    parts.push(achievements)
  }

  return parts.join(' ')
}

/** "🏆 Nyt achievement: Første skridt" – tom tekst hvis listen er tom. */
export function achievementsMessage(achievements: Achievement[]): string {
  if (achievements.length === 0) return ''
  const label = achievements.length === 1 ? 'Nyt achievement' : 'Nye achievements'
  return `🏆 ${label}: ${achievements.map((a) => a.name).join(', ')}.`
}

export const achievementIcons: Record<AchievementType, string> = {
  Level: '⭐',
  TasksCompleted: '✅',
  ItemsBought: '🛍️',
  ChallengesCompleted: '🏁',
}

export const repeatLabels: Record<RecurrenceFrequency, string> = {
  Daily: 'hver dag',
  Weekdays: 'hver hverdag',
  Weekly: 'hver uge',
  Biweekly: 'hver 2. uge',
  Monthly: 'hver måned',
}
