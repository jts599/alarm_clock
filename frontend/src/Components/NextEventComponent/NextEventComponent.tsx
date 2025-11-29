import { useNextAlarmEvent } from '../../hooks/useStateSummary'
import './NextEventComponent.css'
import { AlarmEventInfo, EventType } from '../../../../shared/api/generated'
import { isARealError } from '../../Clients/StateClient'
import { Icon, Icons } from '../Icon'

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
                return Icons.Lights.SUNRISE 
            case EventType.NUMBER_0:
                return Icons.Lights.LIGHTS_OFF// Dim light symbol to indicate light turning off
            default:
                return Icons.Lights.LIGHTS_OFF // Default light off
        }
    }

    const iconName = getEventIcon(nextAlarmEventInfo)

    return (
        <div className={`next-event-component`}>
            <div className="next-event-content">
                <div className="event-icon">
                    <Icon name={iconName} size={48} />
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