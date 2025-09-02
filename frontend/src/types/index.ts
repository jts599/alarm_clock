// WiFi related types for the alarm clock application
export interface WiFiNetwork {
    ssid: string;
    signal: number;
    security: string;
    frequency: number;
}

export interface WiFiConnectionRequest {
    ssid: string;
    password: string;
}

export interface WiFiConnectionResponse {
    success: boolean;
    message?: string;
}

// General API response type
export interface ApiResponse<T = any> {
    success: boolean;
    data?: T;
    message?: string;
    error?: string;
}