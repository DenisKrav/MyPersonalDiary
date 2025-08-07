import { jwtDecode } from 'jwt-decode';
import React, { createContext, useState, useContext, useEffect, type ReactNode } from 'react';

interface AuthContextType {
    isAuthenticated: boolean;
    login: (token: string, rememberMe: boolean) => void;
    logout: () => void;
    token?: string | null;
    userEmail?: string | null;
    userRole?: string | null;
    userId?: string | null;
    userName?: string | null;
    userSurname?: string | null;
}

interface JwtPayload {
    exp: number;
    email?: string;
    sub?: string;
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?: string;
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'?: string;
    userSurname?: string;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [token, setToken] = useState<string | null>(localStorage.getItem('token'));
    const [userRole, setUserRole] = useState<string | null>(null);
    const [userEmail, setUserEmail] = useState<string | null>(null);
    const [userId, setUserId] = useState<string | null>(null);
    const [userName, setUserName] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        const storedToken = localStorage.getItem('token');
        if (storedToken) {
            try {
                const decoded: JwtPayload = jwtDecode(storedToken);
                const expirationTime = decoded.exp * 1000;
                const currentTime = Date.now();

                if (expirationTime < currentTime) {
                    localStorage.removeItem('token');
                    setIsAuthenticated(false);
                    setUserEmail(null);
                    setUserRole(null);
                    setUserId(null);
                    setUserName(null);
                } else {
                    setToken(storedToken);
                    setIsAuthenticated(true);
                    setUserEmail(decoded.email || null);
                    setUserRole(decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || null);
                    setUserId(decoded.sub || null);
                    setUserName(decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || null);
                }
            } catch (error) {
                console.error('Error while decoding token:', error);
                localStorage.removeItem('token');
                setIsAuthenticated(false);
                setUserEmail(null);
                setUserRole(null);
                setUserId(null);
                setUserName(null);
            }
        } else {
            setIsAuthenticated(false);
        }
        setLoading(false);
    }, []);

    const login = (token: string, rememberMe: boolean) => {
        try {
            const decode: JwtPayload = jwtDecode(token);
            setToken(token);

            if (rememberMe) {
                localStorage.setItem('token', token);
            } else {
                localStorage.removeItem('token');
            }

            setIsAuthenticated(true);
            setUserEmail(decode.email || null);
            setUserId(decode.sub || null);
            setUserRole(decode["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || null);
            setUserName(decode['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || null);
        }
        catch (error) {
            console.error('Error during login:', error);
        }
    };

    const logout = () => {
        setToken(null);
        localStorage.removeItem('token');
        setIsAuthenticated(false);
        setUserEmail(null);
        setUserId(null);
        setUserRole(null);
        setUserName(null);
    };

    return (
        <AuthContext.Provider value={{ isAuthenticated, login, logout, token, userEmail, userRole, userId, userName }}>
            {loading ? <div>Loading...</div> : children}
        </AuthContext.Provider>
    );
};

export const useAuth = (): AuthContextType => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};
