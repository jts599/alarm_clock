export function GetAlarmClient(): IAlarmClient {
    return new StubAlarmClient()
}


/// all classes inheriting from this should be static classes
export interface IAlarmClient {
    getNumberOfActiveBulbs(): Promise<number>
    getNextAlarmEventInfo(): Promise<IAlarmEventInfo>
    turnLightOnUntil(nextEventTime: Date): Promise<void>
    removeAlarmOverride(): Promise<void>

}

export interface IAlarmEventInfo {
    nextEventDayOfWeek: string
    nextEventTime: string
    nextEventType: EventType
    isAlarmActive: boolean
}

export enum EventType {
    LightOff = 0,
    Sunrise = 1
}

export class StubAlarmClient implements IAlarmClient {
    async getNumberOfActiveBulbs(): Promise<number> {
        return 3
    }
    
    async getNextAlarmEventInfo(): Promise<IAlarmEventInfo> {
        return {
            nextEventDayOfWeek: 'Monday',
            nextEventTime: '07:00 AM',
            nextEventType: EventType.Sunrise,
            isAlarmActive: true
        }
    }

    async turnLightOnUntil(nextEventTime: Date): Promise<void> {
        // Implementation for turning the light on until the next event time
    }

    async removeAlarmOverride(): Promise<void> {
        // Implementation for removing the alarm override
    }
}

//todo implement a real alarm client that interacts with a backend service  