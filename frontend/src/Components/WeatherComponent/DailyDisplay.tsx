import React from 'react';
import { IDailyForecast, IHourlyForecast } from '../../Clients/Weather/ForecastFetchClient';
import { Icon, Icons } from '../Icon';

export interface ICurrentWeatherDisplayProps {
    singleDayForecast: IDailyForecast
    currentHourlyForecast: IHourlyForecast
}

interface ICurrentWeatherProps {
    iconName: string
    precipProbability: number
    currentTemperature: number
    temperatureHigh: number
    temperatureLow: number
}



export const CurrentWeatherDisplay: React.FC<ICurrentWeatherDisplayProps> = (props) => {
    const { singleDayForecast, currentHourlyForecast } = props;
    const { precipProbability, temperatureHigh, temperatureLow } = singleDayForecast;
    const {icon, temperature} = currentHourlyForecast;
    const tempHighRounded = Math.round(temperatureHigh);
    const tempLowRounded = Math.round(temperatureLow);
    const precipProbabilityRounded = Math.round(precipProbability);
    const currentTemperatureRounded = Math.round(temperature);

    return (<StandaloneForecast 
        iconName={icon} 
        precipProbability={ precipProbabilityRounded} 
        temperatureHigh={tempHighRounded} 
        temperatureLow={tempLowRounded} 
        currentTemperature={currentTemperatureRounded}
    />)
}

export const StandaloneForecast: React.FC<ICurrentWeatherProps> = (props) => {
    const { iconName, precipProbability, currentTemperature, temperatureHigh, temperatureLow } = props;
    return (

        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'end'}}>
            {/*Stack*/}
            <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'center' }}>
                {/*Side by side - Icon and current temp*/}
                

                <div style={{ display: 'flex', flexDirection: 'column', gap: '0' }}>
                    <Icon name={iconName} size={48} />
                    <PrecipDisplay precipProbability={precipProbability} />
                </div>

                <div style={{ fontSize: '2.25rem', fontWeight: 'bold' }}>{currentTemperature}°</div>
                <div style={{ display: 'flex', flexDirection: 'column' }}>
                    <span style={{paddingTop: '3px'}}>H:{temperatureHigh}°</span>
                    <span style={{paddingTop: '3px'}}>L:{temperatureLow}°</span>
                </div>
            </div>
            <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', fontSize: '14px', lineHeight: '1'}}>
                    {/*Side by side - High/Low*/}
                    
                    
                    
            </div>
        </div>
        
    )
}

export const PrecipDisplay: React.FC<{ precipProbability: number }> = ({ precipProbability }) => {
    return (
        <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', paddingRight: '0.5rem', lineHeight: '1'}}>
            {/*Side by side - Precipitation icon and probability*/}
            <div>
                <Icon name={Icons.Weather.RAINDROP} size={12} />
            </div>
            <div style={{paddingTop: '3px'}}>
                {precipProbability}%
            </div>
        </div>
    );
}