import React from 'react';
import { IDailyForecast, IHourlyForecast } from '../../Clients/Weather/ForecastFetchClient';
import { Icon, Icons } from '../Icon';
import { ISize } from '../Icon/Icon';

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
        <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'stretch', justifyItems: 'flex-end' }}>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '0', alignItems: 'center', marginRight: '8px' }}>
                <div style={{marginBottom:'-5px'}}>
                    <Icon name={iconName} size={32}  />
                </div>
                <PrecipDisplay precipProbability={precipProbability} />
            </div>
            
            <div style={{ fontSize: '3rem'}}>
                {currentTemperature}
            </div>
            <div style={{ display: 'flex', flexDirection: 'column',alignItems: 'end', justifyContent: 'center' }}>
                <div style={{justifyItems: 'start', alignItems:'flex-end', fontSize: '0.9rem'}}>
                    <div>{temperatureHigh}</div>
                    <div>{temperatureLow}</div>
                </div>
            </div>

        </div>
        
    )
}

export const PrecipDisplay: React.FC<{ precipProbability: number }> = ({ precipProbability }) => {
    return (
        <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', lineHeight: '1'}}>
            {/*Side by side - Precipitation icon and probability*/}
            <div>
                <Icon name={Icons.Weather.RAINDROP} size={12} />
            </div>
            <div style={{paddingTop: '3px', fontSize: '0.75rem'}}>
                {precipProbability}%
            </div>
        </div>
    );
}