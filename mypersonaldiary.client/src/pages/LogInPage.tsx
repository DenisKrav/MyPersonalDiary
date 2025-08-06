import { Layout, Form, Input, Button, Checkbox } from 'antd';
import { useNavigate } from 'react-router-dom';
import SiteFooter from '../components/SiteFooter';
import SiteHeader from '../components/SiteHeader';
import { useAuth } from '../context/AuthContext';
import type { LoginModel } from '../api/Models/Login/LoginModel';
import { useLoginUser } from '../api/Queries/Login/useLoginUser';

const { Content } = Layout;

const LogInPage = () => {
    const [form] = Form.useForm();
    const navigate = useNavigate();
    const { login } = useAuth();
    const { mutateAsync: loginUser, isPending } = useLoginUser();

    const onFinish = async (values: any) => {
        const loginModel: LoginModel = {
            email: values.email,
            password: values.password,
        };

        try {
            const token = await loginUser(loginModel);
            login(token, values.remember);
            navigate('/');
        } catch (error: any) {
            console.error(error);
        }
    };

    const onFinishFailed = (errorInfo: any) => {
        console.log('Failed:', errorInfo);
    };

    return (
        <Layout style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
            <SiteHeader />

            <Content style={{ flex: 1, display: 'flex', justifyContent: 'center', alignItems: 'center', backgroundColor: '#f5f5f5' }}>
                <div style={{ maxWidth: '400px', width: '100%', padding: '20px', backgroundColor: '#fff', borderRadius: '8px' }}>
                    <h2 style={{ textAlign: 'center', marginBottom: '20px' }}>Login</h2>

                    <Form
                        form={form}
                        name="login"
                        initialValues={{ remember: true }}
                        onFinish={onFinish}
                        onFinishFailed={onFinishFailed}
                    >
                        <Form.Item
                            name="email"
                            rules={[
                                { required: true, message: "Email required" },
                                { type: 'email', message: "Invalid email" },
                            ]}
                        >
                            <Input placeholder={"Emai"} />
                        </Form.Item>

                        <Form.Item
                            name="password"
                            rules={[{ required: true, message: "Password required" }]}
                        >
                            <Input.Password placeholder={"Password"} />
                        </Form.Item>

                        <Form.Item name="remember" valuePropName="checked">
                            <Checkbox>{"Remember me"}</Checkbox>
                        </Form.Item>


                        <Form.Item>
                            <Button type="primary" htmlType="submit" loading={isPending} style={{ width: '100%' }}>
                                {"Login"}
                            </Button>
                        </Form.Item>
                    </Form>
                </div>
            </Content>

            <SiteFooter />
        </Layout>
    );
};

export default LogInPage;
