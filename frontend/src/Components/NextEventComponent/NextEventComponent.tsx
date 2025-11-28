import { useNextAlarmEvent } from '../../hooks/useStateSummary'
import './NextEventComponent.css'
import { AlarmEventInfo, EventType } from '../../../../shared/api/generated'
import { isARealError } from '../../Clients/StateClient'

export const NextEventComponent: React.FC = () => {
    const { data: eventInfo, isLoading, isError, error } = useNextAlarmEvent(1000)

    if (isLoading) {
        return (
            <div className="next-event-component loading">
                <div className="next-event-loading">Loading next event...</div>
            </div>
        )
    }

    if (isARealError(isError,error) || eventInfo === undefined) {
        return (
            <></>
        )
    }

    const nextAlarmEventInfo: AlarmEventInfo = eventInfo

    const getEventIcon = (eventInfo: AlarmEventInfo): string => {
        switch (eventInfo.nextEventType) {
            case EventType.NUMBER_1:
                return '🌅' //TODO: Replace with actual icon
            case EventType.NUMBER_0:
                return '�' // Dim light symbol to indicate light turning off
            default:
                return '⏰'
        }
    }

    return (
        <div className={`next-event-component`}>
            <div className="next-event-content">
                <div className="event-icon">
                    {getEventIcon(eventInfo)}
                </div>
                <div>
                    
                    <div className="event-time">
                        {eventInfo.nextEventTime}
                    </div>
                    
                    <div className="event-day">
                        {eventInfo.nextEventDayOfWeek}
                    </div>
                </div>
            </div>
        </div>
    )
}

export default NextEventComponent