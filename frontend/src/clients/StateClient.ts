import { AlarmStateApi, Configuration, ResponseError } from '../../../shared/api/generated'
import type { AlarmStateSummary } from '../../../shared/api/generated/models/AlarmStateSummary'

const apiConfig = new Configuration({ basePath: '' })
export const alarmApi = new AlarmStateApi(apiConfig)

export async function fetchStateSummary(): Promise<AlarmStateSummary> {
  try {
    return await alarmApi.apiAlarmStateStateSummaryGet()
  } catch (e: any) {
    // The generated client throws ResponseError on non-2xx responses.
    if (e instanceof ResponseError && e.response && e.response.status === 304) {
      return Promise.reject(new Error('Not Modified'))
    }
    throw e
  }
}

/**
 * Utility to check if an error is a Not Modified error
 * @param error 
 * @returns 
 */
export function isNotModifiedError(error?: Error | null): boolean {
  return error?.message === 'Not Modified'
}

/**
 * Helper to determine if an error is a "real" error (not a Not Modified error)
 * @param isError - IsError flag
 * @param error - error result
 * @returns 
 */
export function isARealError(isError: boolean, error?: Error | null): boolean {
  return isError && !isNotModifiedError(error)
}
