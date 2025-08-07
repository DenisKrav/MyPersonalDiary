import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import type { JSX } from 'react';

const PublicOnlyRoute = ({ children }: { children: JSX.Element }) => {
    const { isAuthenticated } = useAuth();

    return isAuthenticated ? <Navigate to="/admin" replace /> : children;
};

export default PublicOnlyRoute;
