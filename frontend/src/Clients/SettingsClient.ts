

export function GetSettingsClient(): ISettingsClient {
    return new StubSettingsClient()
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