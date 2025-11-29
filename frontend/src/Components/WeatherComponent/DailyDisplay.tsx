import React from 'react';
import { IDailyForecast, IHourlyForecast } from '../../Clients/Weather/ForecastFetchClient';
import { Icon, Icons } from '../Icon';

export enum DailyDisplayMode {
    StandaloneForecast,
    AsPartOfWeeklyForecast
}

export interface ISingleDayDisplayProps {
    singleDayForecast: IDailyForecast
    displayMode: DailyDisplayMode
}

interface ISingleDayForecastProps {
    iconName: string
    precipProbability: number
    temperatureHigh: number
    temperatureLow: number
    dayOfWeek?: string
    dateString?: string
}



export const SingleDayDisplay: React.FC<ISingleDayDisplayProps> = (props) => {
    const { singleDayForecast, displayMode } = props;
    const { icon, precipProbability, temperatureHigh, temperatureLow, date } = singleDayForecast;
    const tempHighRounded = Math.round(temperatureHigh);
    const tempLowRounded = Math.round(temperatureLow);
    const precipProbabilityRounded = Math.round(precipProbability);
    const dateString = date.toLocaleDateString(undefined, { month: '2-digit', day: '2-digit' });
    const dayOfWeek = date.toLocaleDateString(undefined, { weekday: 'short' });

    if (displayMode === DailyDisplayMode.StandaloneForecast) {
        return (<StandaloneForecast 
            iconName={icon} 
            precipProbability={ precipProbabilityRounded} 
            temperatureHigh={tempHighRounded} 
            temperatureLow={tempLowRounded} 
            dateString={dateString} 
            dayOfWeek={dayOfWeek} 
            />)
    } else if (displayMode === DailyDisplayMode.AsPartOfWeeklyForecast) {
        return (<SingleDayForecastAsPartOfWeekly 
            iconName={icon} 
            precipProbability={precipProbabilityRounded} 
            temperatureHigh={tempHighRounded} 
            temperatureLow={tempLowRounded} 
            dateString={dateString} 
            dayOfWeek={dayOfWeek} 
            />)
    }
    return (<></>)
}

export const StandaloneForecast: React.FC<ISingleDayForecastProps> = (props) => {
    const { iconName, precipProbability, temperatureHigh, temperatureLow, dayOfWeek, dateString } = props;
    return (
        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '0.5rem' }}>
            <div>
                {dayOfWeek}, {dateString}
            </div>
            <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', gap: '1rem' }}>
                {/*Side by side*/}
                <div>
                    <Icon name={iconName} size={48} />
                </div>
                <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                    {/*Stack*/}
                    <div>
                        {temperatureHigh}° / {temperatureLow}°
                    </div>
                    <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', gap: '0.25rem' }}>
                        {/*Side by side*/}
                        <div>
                            <Icon name={Icons.Weather.RAINDROPS} size={16} />
                        </div>
                        <div>{precipProbability}%</div>
                    </div>
                </div>
            </div>
        </div>
        
    )
}

/**
 * This will need to be updated when used in weekly forecast to be more compact.
 * @param props 
 * @returns 
 */
export const SingleDayForecastAsPartOfWeekly: React.FC<ISingleDayForecastProps> = (props) => {
    const { iconName, precipProbability, temperatureHigh, temperatureLow, dayOfWeek, dateString } = props;
    return (
        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '0.5rem' }}>
            <div>
                {dayOfWeek}, {dateString}
            </div>
            <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', gap: '1rem' }}>
                {/*Side by side*/}
                <div>
                    <Icon name={iconName} size={48} />
                </div>
                <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                    {/*Stack*/}
                    <div>
                        {temperatureHigh}° / {temperatureLow}°
                    </div>
                    <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', gap: '0.25rem' }}>
                        {/*Side by side*/}
                        <div>
                            <Icon name={Icons.Weather.RAINDROPS} size={8} />
                        </div>
                        <div>{precipProbability}%</div>
                    </div>
                </div>
            </div>
        </div>
    )
}