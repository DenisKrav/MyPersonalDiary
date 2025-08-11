import { Layout, Menu, Dropdown, Avatar, type MenuProps, Modal, Button } from 'antd';
import { UserOutlined, LogoutOutlined, LoginOutlined, DeleteOutlined } from '@ant-design/icons';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { toast } from 'sonner';
import { useDeleteUser } from '../api/Queries/User/useDeleteUser';
import { UserRole } from '../enums/UserRole';
import { useState } from 'react';

const { Header } = Layout;

const SiteHeader = () => {
    const navigate = useNavigate();
    const location = useLocation();

    const { isAuthenticated, token, userRole, logout } = useAuth();
    const { mutateAsync: deleteUser, isPending: isDeleting } = useDeleteUser(token ?? "");

    const [isDeleteOpen, setIsDeleteOpen] = useState(false);

    const menuItems = [
        { key: '/', label: 'Notes' },
        ...(isAuthenticated && userRole === UserRole.Admin
            ? [{ key: '/admin', label: 'Admin' }]
            : [])
    ];

    const activeKey = menuItems.find(item => item.key === location.pathname)?.key;

    const userMenuItems: MenuProps['items'] = isAuthenticated
        ? [
            ...(userRole !== UserRole.Admin
                ? [
                    {
                        key: 'delete',
                        icon: <DeleteOutlined />,
                        label: <span style={{ color: '#ff4d4f' }}>Delete account</span>,
                        danger: true,
                    } as const,
                    { type: 'divider' as const },
                ]
                : []),
            {
                key: 'logout',
                icon: <LogoutOutlined />,
                label: 'Log out',
            },
        ]
        : [
            {
                key: '/login',
                icon: <LoginOutlined />,
                label: <Link to="/login">Login</Link>,
            },
        ];

    const handleConfirmDelete = async () => {
        try {
            await deleteUser();
            toast.success('Account deletion requested');
            setIsDeleteOpen(false);
            logout();
            navigate('/login');
        } catch (e: any) {
            toast.error(e?.message ?? 'Delete failed');
        }
    };

    const handleUserMenuClick: MenuProps['onClick'] = ({ key }) => {
        if (key === "logout") {
            logout();
            navigate('/');
        } else if (key === 'delete') {
            setIsDeleteOpen(true);
            return;
        }
        else {
            navigate(key);
        }
    };

    return (
        <Header style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', backgroundColor: "#638262" }}>
            <Menu
                theme="dark"
                mode="horizontal"
                selectedKeys={[activeKey ?? '/']}
                onClick={({ key }) => navigate(key)}
                items={menuItems}
                style={{ backgroundColor: "#638262", flex: 1 }}

            />
            <div style={{ marginLeft: 'auto', display: 'flex', alignItems: 'center' }}>
                <Dropdown menu={{ items: userMenuItems, onClick: handleUserMenuClick }} placement="bottomRight">
                    <Avatar style={{ backgroundColor: '#87d068', cursor: 'pointer' }} icon={<UserOutlined />} />
                </Dropdown>
            </div>

            <Modal
                open={isDeleteOpen}
                title="Delete account?"
                onCancel={() => setIsDeleteOpen(false)}
                footer={[
                    <Button key="cancel" onClick={() => setIsDeleteOpen(false)}>Cancel</Button>,
                    <Button
                        key="delete"
                        danger
                        type="primary"
                        loading={isDeleting}
                        onClick={handleConfirmDelete}
                    >
                        Delete
                    </Button>,
                ]}
            >
                Your account will be marked for deletion and you will be signed out immediately.
                This action cannot be undone.
            </Modal>
        </Header>
    );
};

export default SiteHeader;
