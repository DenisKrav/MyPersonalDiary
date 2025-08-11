import { Layout, Form, Input, Button } from 'antd';
import SiteHeader from '../components/SiteHeader';
import SiteFooter from '../components/SiteFooter';
import { toast } from 'sonner';
import { useNavigate } from 'react-router-dom';
import { useRestoreAccount } from '../api/Queries/User/useRestoreAccount';
// TODO: заміни на свій запит
// import { useRestoreAccount } from '../api/Queries/User/useRestoreAccount';

const { Content } = Layout;

const RestoreAccountPage = () => {
    const [form] = Form.useForm();
    const navigate = useNavigate();
    const { mutateAsync: restore, isPending } = useRestoreAccount();

    const onFinish = async (values: any) => {
        try {
            await restore({ email: values.email });
            toast.success('Your account will be restored.');
            navigate('/login');
        } catch (e: any) {
            toast.error(e?.message ?? 'Restore failed');
        }
    };

    return (
        <Layout style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
            <SiteHeader />

            <Content style={{ flex: 1, display: 'flex', justifyContent: 'center', alignItems: 'center', backgroundColor: '#f5f5f5' }}>
                <div style={{ maxWidth: '400px', width: '100%', padding: '20px', backgroundColor: '#fff', borderRadius: '8px' }}>
                    <h2 style={{ textAlign: 'center', marginBottom: '20px' }}>Restore Account</h2>

                    <Form
                        form={form}
                        name="restoreAccount"
                        onFinish={onFinish}
                    >
                        <Form.Item
                            name="email"
                            rules={[
                                { required: true, message: "Email required" },
                                { type: 'email', message: "Invalid email" },
                            ]}
                        >
                            <Input placeholder="Email" />
                        </Form.Item>

                        <Form.Item>
                            <Button type="primary" htmlType="submit" loading={isPending} style={{ width: '100%' }}>
                                Restore
                            </Button>
                        </Form.Item>
                    </Form>
                </div>
            </Content>

            <SiteFooter />
        </Layout>
    );
};

export default RestoreAccountPage;
