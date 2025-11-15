import { AlarmStateApi, Configuration, ResponseError } from '../../../shared/api/generated'
import type { AlarmStateSummary } from '../../../shared/api/generated/models/AlarmStateSummary'

const apiConfig = new Configuration({ basePath: '' })
const alarmApi = new AlarmStateApi(apiConfig)

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
