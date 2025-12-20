import React from 'react';
import { IHourlyForecast } from '../../Clients/Weather/ForecastFetchClient';

export interface ISingleHourDisplayProps {
    hourlyForecast: IHourlyForecast
}

export const singleHourDisplay: React.FC<ISingleHourDisplayProps> = (props) => {
    const { hourlyForecast } = props;
    return (<></>)
}