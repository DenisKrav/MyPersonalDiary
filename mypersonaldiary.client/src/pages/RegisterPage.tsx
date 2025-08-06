import { Layout, Form, Input, Button, message } from 'antd';
import { useNavigate } from 'react-router-dom';
import SiteFooter from '../components/SiteFooter';
import SiteHeader from '../components/SiteHeader';
import { useAuth } from '../context/AuthContext';
import { useRegisterUser } from '../api/Queries/Register/useRegisterUser';
import type { NewUserModel } from '../api/Models/User/NewUserModel';

const { Content } = Layout;

const RegisterPage = () => {
    const [form] = Form.useForm();
    const navigate = useNavigate();
    const { login } = useAuth();
    const { mutateAsync: registerUser, isPending } = useRegisterUser();
    // const [rememberMe] = useState(true); // або окремий чекбокс, якщо хочеш

    const onFinish = async (values: any) => {
        const newCustomer: NewUserModel = {
            nickname: values.nickname,
            email: values.email,
            password: values.password,
        };

        try {
            const token = await registerUser(newCustomer);
            // login(token, rememberMe);
            login(token, false);
            message.success("Registration successful");
            navigate('/');
        } catch (error: any) {
            console.error(error);
            message.error(error.message || "Registration failed");
        }
    };

    const onFinishFailed = (errorInfo: any) => {
        console.log('Failed:', errorInfo);
    };

    return (
        <Layout style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
            <SiteHeader />

            <Content style={{ flex: 1, display: 'flex', justifyContent: 'center', alignItems: 'center', backgroundColor: '#f5f5f5', margin: "20px 0px" }}>
                <div style={{ maxWidth: '400px', width: '100%', padding: '20px', backgroundColor: '#fff', borderRadius: '8px' }}>
                    <h2 style={{ textAlign: 'center', marginBottom: '20px' }}>Register page</h2>

                    <Form
                        form={form}
                        name="register"
                        initialValues={{ remember: true }}
                        onFinish={onFinish}
                        onFinishFailed={onFinishFailed}
                    >
                        <Form.Item
                            name="nickname"
                            rules={[{ required: true, message: "Nickname is required" }]}
                        >
                            <Input placeholder="Input nickname" />
                        </Form.Item>

                        <Form.Item
                            name="email"
                            rules={[
                                { required: true, message: "Email is required" },
                                { type: 'email', message: "Invalid email" },
                              ]}
                        >
                            <Input placeholder="Input email" />
                        </Form.Item>

                        <Form.Item
                            name="password"
                            rules={[
                                {
                                    required: true,
                                    message: "Password is required",
                                },
                                {
                                    min: 6,
                                    message: "Password must be at least 6 characters",
                                },
                                {
                                    pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$/,
                                    message: "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character",
                                },
                            ]}
                        >
                            <Input.Password placeholder="Input password" />
                        </Form.Item>

                        <Form.Item
                            name="confirm"
                            dependencies={['password']}
                            hasFeedback
                            rules={[
                                { required: true, message: "Confirm password is required" },
                                ({ getFieldValue }) => ({
                                    validator(_, value) {
                                        if (!value || getFieldValue('password') === value) {
                                            return Promise.resolve();
                                        }
                                        return Promise.reject(new Error("Posswords do not match"));
                                    },
                                }),
                            ]}
                        >
                            <Input.Password placeholder="Input confirm password" />
                        </Form.Item>

                        <Form.Item>
                            <Button type="primary" htmlType="submit" loading={isPending} style={{ width: '100%' }}>
                                Register
                            </Button>
                        </Form.Item>
                    </Form>
                </div>
            </Content>

            <SiteFooter />
        </Layout>
    );
};

export default RegisterPage;
