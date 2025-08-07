import { Layout, Menu, Dropdown, Avatar, type MenuProps } from 'antd';
import { UserOutlined, LogoutOutlined, LoginOutlined } from '@ant-design/icons';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { UserRole } from '../enums/UserRole ';

const { Header } = Layout;

const SiteHeader = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { isAuthenticated, userRole, logout } = useAuth();

    const menuItems = [
        { key: '/', label: 'Short URLs' },
        ...(isAuthenticated && userRole === UserRole.Admin
            ? [{ key: '/admin', label: 'Admin' }]
            : [])
    ];

    const activeKey = menuItems.find(item => item.key === location.pathname)?.key;

    const userMenuItems: MenuProps["items"] = [
        ...(isAuthenticated
            ? [
                {
                    key: "logout",
                    icon: <LogoutOutlined />,
                    label: "Log out",
                },
            ]
            : [
                {
                    key: "/login",
                    icon: <LoginOutlined />,
                    label: <Link to="/login">Login</Link>,
                },
            ]),
    ];

    const handleUserMenuClick: MenuProps['onClick'] = ({ key }) => {
        if (key === "logout") {
            logout();
            navigate('/');
        } else {
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
        </Header>
    );
};

export default SiteHeader;
