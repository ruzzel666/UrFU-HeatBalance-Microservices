import { Typography, Card, Tag, Empty } from 'antd';
import { ExperimentOutlined, ClockCircleOutlined } from '@ant-design/icons';

const { Title, Paragraph } = Typography;

export default function DrumDryerPage() {
  return (
    <div style={{ maxWidth: 1100, margin: '0 auto' }}>
      <div className="animate-fade-in-up">
        <div style={{ marginBottom: 24, display: 'flex', alignItems: 'center', gap: 12 }}>
          <div
            style={{
              width: 42,
              height: 42,
              borderRadius: 10,
              background: 'linear-gradient(135deg, #1890ff, #36cfc9)',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              color: '#fff',
              fontSize: 20,
              boxShadow: '0 4px 12px rgba(24, 144, 255, 0.3)',
            }}
          >
            <ExperimentOutlined />
          </div>
          <div>
            <Title level={3} style={{ margin: 0 }}>Сушильный барабан</Title>
            <Tag color="processing">В разработке</Tag>
          </div>
        </div>
      </div>

      <Card className="animate-fade-in-up delay-1">
        <Empty
          image={<ClockCircleOutlined style={{ fontSize: 64, color: '#1890ff' }} />}
          description={
            <div style={{ marginTop: 8 }}>
              <Paragraph strong>Страница в разработке — Итерация 4</Paragraph>
              <Paragraph type="secondary">
                Форма ввода параметров сушильного барабана будет доступна после получения
                спецификации API от backend-разработчика.
              </Paragraph>
            </div>
          }
        />
      </Card>
    </div>
  );
}
