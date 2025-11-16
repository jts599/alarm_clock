import { useQuery, UseQueryResult, UseQueryOptions } from '@tanstack/react-query'
import { fetchStateSummary } from '../clients/StateClient'
import { AlarmStateSummary, AlarmEventInfo, WeatherResponse, LightOverrideState } from '../../../shared/api/generated/models'

export function useStateSummary(pollIntervalMs = 1000): UseQueryResult<AlarmStateSummary, Error> {
  return useQuery<AlarmStateSummary, Error, AlarmStateSummary, readonly ['stateSummary']>(
    ({
      queryKey: ['stateSummary'] as const,
      queryFn: fetchStateSummary,
      refetchInterval: pollIntervalMs,
      staleTime: pollIntervalMs,
      cacheTime: 5 * 60 * 1000,
      keepPreviousData: true,
    } as UseQueryOptions<AlarmStateSummary, Error, AlarmStateSummary, readonly ['stateSummary']>)
  )
}

export function useActiveBulbsCount(pollIntervalMs = 1000): UseQueryResult<number | undefined, Error> {
  return useQuery<AlarmStateSummary, Error, number | undefined, readonly ['stateSummary']>(
    ({
      queryKey: ['stateSummary'] as const,
      queryFn: fetchStateSummary,
      refetchInterval: pollIntervalMs,
      select: (data: AlarmStateSummary | undefined) => data?.numberOfActiveBulbs,
      keepPreviousData: true,
    } as UseQueryOptions<AlarmStateSummary, Error, number | undefined, readonly ['stateSummary']>)
  )
}

export function useNextAlarmEvent(pollIntervalMs = 1000): UseQueryResult<AlarmEventInfo | undefined, Error> {
  return useQuery<AlarmStateSummary, Error, AlarmEventInfo | undefined, readonly ['stateSummary']>(
    ({
      queryKey: ['stateSummary'] as const,
      queryFn: fetchStateSummary,
      refetchInterval: pollIntervalMs,
      select: (data: AlarmStateSummary | undefined) => data?.nextAlarmEvent as AlarmEventInfo | undefined,
      keepPreviousData: true,
    } as UseQueryOptions<AlarmStateSummary, Error, AlarmEventInfo | undefined, readonly ['stateSummary']>)
  )
}

export function useWeather(pollIntervalMs = 30_000): UseQueryResult<WeatherResponse | undefined, Error> {
  return useQuery<AlarmStateSummary, Error, WeatherResponse | undefined, readonly ['stateSummary']>(
    ({
      queryKey: ['stateSummary'] as const,
      queryFn: fetchStateSummary,
      refetchInterval: pollIntervalMs,
      select: (data: AlarmStateSummary | undefined) => data?.weatherForecast as WeatherResponse | undefined,
      keepPreviousData: true,
    } as UseQueryOptions<AlarmStateSummary, Error, WeatherResponse | undefined, readonly ['stateSummary']>)
  )
}

export function useOverrideStatus(pollIntervalMs = 1000): UseQueryResult<LightOverrideState | undefined, Error> {
  return useQuery<AlarmStateSummary, Error, LightOverrideState | undefined, readonly ['stateSummary']>(
    ({
      queryKey: ['stateSummary'] as const,
      queryFn: fetchStateSummary,
      refetchInterval: pollIntervalMs,
      select: (data: AlarmStateSummary | undefined) => data?.lightOverrideState as LightOverrideState | undefined,
      keepPreviousData: true,
    } as UseQueryOptions<AlarmStateSummary, Error, LightOverrideState | undefined, readonly ['stateSummary']>)
  )
}

  export function useCurrentTime(pollIntervalMs = 1000): UseQueryResult<Date | undefined, Error> {
    return useQuery<AlarmStateSummary, Error, Date | undefined, readonly ['stateSummary']>(
      ({
        queryKey: ['stateSummary'] as const,
        queryFn: fetchStateSummary,
        refetchInterval: pollIntervalMs,
        select: (data: AlarmStateSummary | undefined) => data?.currentTime as Date | undefined,
        keepPreviousData: true,
      } as UseQueryOptions<AlarmStateSummary, Error, Date | undefined, readonly ['stateSummary']>)
    )
}
