import { Layout } from 'antd';

const { Footer } = Layout;

const SiteFooter = () => {
    return (
        <Footer
            style={{
                textAlign: 'center',
                backgroundColor: '#638262',
                color: 'white',
                padding: '10px 50px',
                fontSize: '14px',
            }}
        >
            <div>
                My personal diary
            </div>
        </Footer>
    );
};

export default SiteFooter;
