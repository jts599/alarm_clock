import { BrightnessApi, Configuration } from '../../../shared/api/generated'

const apiConfig = new Configuration({ basePath: '' })
const brightnessApi = new BrightnessApi(apiConfig)

/**
 * Set brightness based on the time of day.
 * Uses higher brightness during daytime (6 AM - 8 PM) and lower brightness during nighttime.
 */
export async function setAppropriateBrightness(): Promise<void> {
  const now = new Date()
  const hour = now.getHours()
  
  // Daytime: 6 AM to 8 PM - use full brightness (100)
  // Nighttime: 8 PM to 6 AM - use lower brightness (20)
  const brightness = (hour >= 6 && hour < 20) ? 100 : 20
  
  await brightnessApi.apiBrightnessPost({
    setBrightnessRequest: { brightness }
  })
}

/**
 * Set brightness to dim (1).
 */
export async function setDimBrightness(): Promise<void> {
  await brightnessApi.apiBrightnessPost({
    setBrightnessRequest: { brightness: 1 }
  })
}
