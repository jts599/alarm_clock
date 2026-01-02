

import { AlarmSettingsApi, UserSettings, Configuration } from '../../../shared/api/generated'

export function GetSettingsClient(): ISettingsClient {
    return new RealSettingsClient()
}

export interface ISettingsClient {
    getUserSettings(): Promise<IUserSettings>
    updateUserSettings(settings: IUserSettings): Promise<void>
}

export interface IUserSettings {
    AlarmTimeInMinutesSinceMidnight: number
    transitionMinutes: number
    turnOffAfterMinutes: number
    enabledDaysOfWeek: string[]
}


export class RealSettingsClient implements ISettingsClient {
    private api: AlarmSettingsApi

    constructor() {
        const config = new Configuration({
            basePath: ''
        })
        this.api = new AlarmSettingsApi(config)
    }

    async getUserSettings(): Promise<IUserSettings> {
        const dto = await this.api.apiAlarmSettingsGet()
        return this.mapDtoToUserSettings(dto)
    }

    async updateUserSettings(settings: IUserSettings): Promise<void> {
        const dto = this.mapUserSettingsToDto(settings)
        await this.api.apiAlarmSettingsPut({ userSettings: dto })
    }

    private mapDtoToUserSettings(dto: UserSettings): IUserSettings {
        return {
            AlarmTimeInMinutesSinceMidnight: dto.alarmTimeInMinutesSinceMidnight ?? 0,
            transitionMinutes: dto.transitionMinutes ?? 0,
            turnOffAfterMinutes: dto.turnOffAfterMinutes ?? 0,
            enabledDaysOfWeek: dto.enabledDaysOfWeek ?? []
        }
    }

    private mapUserSettingsToDto(settings: IUserSettings): UserSettings {
        return {
            alarmTimeInMinutesSinceMidnight: settings.AlarmTimeInMinutesSinceMidnight,
            transitionMinutes: settings.transitionMinutes,
            turnOffAfterMinutes: settings.turnOffAfterMinutes,
            enabledDaysOfWeek: settings.enabledDaysOfWeek
        }
    }
}

export class StubSettingsClient implements ISettingsClient {
    private settings: IUserSettings = {
        AlarmTimeInMinutesSinceMidnight: 420, // 7:00 AM
        transitionMinutes: 30,
        turnOffAfterMinutes: 60,
        enabledDaysOfWeek: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday']
    }
    
    async getUserSettings(): Promise<IUserSettings> {
        return this.settings
    }

    async updateUserSettings(settings: IUserSettings): Promise<void> {
        this.settings = settings
    }
}