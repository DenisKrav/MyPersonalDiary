import { Layout, Form, Input, Button, Spin, Typography } from 'antd';
import { useNavigate, useSearchParams } from 'react-router-dom';
import SiteFooter from '../components/SiteFooter';
import SiteHeader from '../components/SiteHeader';
import { useAuth } from '../context/AuthContext';
import { useRegisterUser } from '../api/Queries/Register/useRegisterUser';
import type { NewUserModel } from '../api/Models/User/NewUserModel';
import { useEffect, useState } from 'react';
import { useCheckInviteCode } from '../api/Queries/Invite/useCheckInviteCode';
import { toast } from 'sonner';
import ReCAPTCHA from 'react-google-recaptcha';

const { Content } = Layout;

const RegisterPage = () => {
    const RECAPTCHA_SITE_KEY = import.meta.env.VITE_RECAPTCHA_SITE_KEY;
    const { Title } = Typography;

    const [form] = Form.useForm();
    const navigate = useNavigate();
    const { login } = useAuth();
    const [searchParams] = useSearchParams();

    const code = searchParams.get("code");

    const [emailFromInvite, setEmailFromInvite] = useState<string | null>(null);
    const [captchaToken, setCaptchaToken] = useState<string | null>(null);

    const { mutateAsync: registerUser, isPending } = useRegisterUser();
    const { mutateAsync: checkInviteCode, isPending: checkingCode } = useCheckInviteCode();

    useEffect(() => {
        const validate = async () => {
            if (!code) {
                toast.error("Invite is not found");
                return;
            }

            try {
                const resultEmail = await checkInviteCode({ code: code });
                setEmailFromInvite(resultEmail);
            } catch (err: any) {
                toast.error("Invite code is not valid");
            }
        };

        validate();
    }, [code, checkInviteCode]);

    useEffect(() => {
        if (emailFromInvite) {
            form.setFieldsValue({ email: emailFromInvite });
        }
    }, [emailFromInvite, form]);

    const onFinish = async (values: any) => {
        if (!captchaToken) {
            toast.error("Please complete the reCAPTCHA");
            return;
        }

        const newCustomer: NewUserModel = {
            nickname: values.nickname,
            email: values.email,
            password: values.password,
        };

        try {
            const token = await registerUser(newCustomer);
            login(token, false);
            toast.success("Registration successful");
            navigate('/');
        } catch (error: any) {
            console.error(error);
            toast.error(error.message || "Registration failed");
        }
    };

    const onFinishFailed = (errorInfo: any) => {
        console.log('Failed:', errorInfo);
    };

    if (checkingCode) {
        return (
            <div style={{ height: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <Spin size="large">
                    <div style={{ width: "100%", height: "100%" }} />
                </Spin>
            </div>
        );
    }

    if (!emailFromInvite) {
        return (
            <div style={{ textAlign: 'center', marginTop: '100px' }}>
                <Title level={3}>Invite is not valid</Title>
            </div>
        );
    }

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

                        <Form.Item name="email">
                            <Input disabled readOnly />
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

                        <Form.Item
                            shouldUpdate
                            style={{ textAlign: 'center' }}
                        >
                            <ReCAPTCHA
                                sitekey={RECAPTCHA_SITE_KEY}
                                onChange={(token: any) => setCaptchaToken(token)}
                                onExpired={() => setCaptchaToken(null)}
                            />
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
