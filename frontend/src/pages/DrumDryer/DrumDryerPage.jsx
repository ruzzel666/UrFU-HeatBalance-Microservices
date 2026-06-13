import { useState } from 'react';
import { calculateDrumDryer } from '../../api/drumDryerApi';
import DrumDryerResultsView from './DrumDryerResultsView';
import {
  Form,
  InputNumber,
  Button,
  Tabs,
  Card,
  Typography,
  Space,
  Tag,
  Divider,
  Row,
  Col,
  message,
  Tooltip,
} from 'antd';
import {
  ExperimentOutlined,
  FireOutlined,
  ColumnWidthOutlined,
  ClockCircleOutlined,
  RadiusSettingOutlined,
  SendOutlined,
  InfoCircleOutlined,
  ReloadOutlined,
} from '@ant-design/icons';
import './DrumDryerPage.css';

const { Title, Text } = Typography;

/* ===== Default values ===== */
const defaultValues = {
  // Материал
  dryMaterialMass: 8000,
  initialMoisture: 12,
  finalMoisture: 2,
  initialMaterialTemp: 15,
  materialBulkDensity: 1400,
  materialHeatCapacity: 0.84,

  // Топливо и воздух
  airFlowCoefficient: 1.15,
  airTemperature: 20,
  dryingAgentTempIn: 600,
  dryingAgentTempOut: 120,
  recirculateTemp: 180,
  deltaAlpha: 0.25,

  // Геометрия барабана
  drumDiameter: 2.0,
  drumLength: 12.0,
  drumWallThickness: 0.012,
  insulationThickness: 0.05,
  drumInclination: 3,
  fillFactor: 0.15,

  // Время и производительность
  dryingTime: 3600,
  rotationSpeed: 3,
  materialFeedRate: 0.5,
};

/* ===== Field definitions ===== */
const materialFields = [
  { name: 'dryMaterialMass', label: 'Масса сухого материала', unit: 'кг', symbol: 'Gс.м', min: 0.1, max: 1000000 },
  { name: 'initialMoisture', label: 'Начальная влажность', unit: '%', symbol: 'W₁нач', min: 0, max: 100 },
  { name: 'finalMoisture', label: 'Конечная влажность', unit: '%', symbol: 'W₂кон', min: 0, max: 100 },
  { name: 'initialMaterialTemp', label: 'Начальная температура материала', unit: '°С', symbol: 'tм.нач', min: -50, max: 500 },
  { name: 'materialBulkDensity', label: 'Насыпная плотность материала', unit: 'кг/м³', symbol: 'ρнас', min: 50, max: 5000, step: 10 },
  { name: 'materialHeatCapacity', label: 'Удельная теплоёмкость материала', unit: 'кДж/(кг·°С)', symbol: 'cм', min: 0.1, max: 5, step: 0.01 },
];

const fuelFields = [
  { name: 'airFlowCoefficient', label: 'Коэффициент расхода воздуха', unit: '', symbol: 'α', min: 0.1, max: 10, step: 0.01 },
  { name: 'airTemperature', label: 'Температура воздуха', unit: '°С', symbol: 'tвозд', min: -50, max: 100 },
  { name: 'dryingAgentTempIn', label: 'Температура сушильного агента на входе', unit: '°С', symbol: 'tс.а.вх', min: 50, max: 1000 },
  { name: 'dryingAgentTempOut', label: 'Температура сушильного агента на выходе', unit: '°С', symbol: 'tс.а.вых', min: 20, max: 500 },
  { name: 'recirculateTemp', label: 'Температура рециркулята', unit: '°С', symbol: 'tрец', min: 0, max: 500 },
  { name: 'deltaAlpha', label: 'Увеличение коэф. расхода воздуха (рец.)', unit: '', symbol: 'Δα', min: 0, max: 5, step: 0.01 },
];

const geometryFields = [
  { name: 'drumDiameter', label: 'Внутренний диаметр барабана', unit: 'м', symbol: 'D', min: 0.3, max: 5, step: 0.1 },
  { name: 'drumLength', label: 'Длина барабана', unit: 'м', symbol: 'L', min: 1, max: 30, step: 0.1 },
  { name: 'drumWallThickness', label: 'Толщина стенки барабана', unit: 'м', symbol: 'Sст', min: 0.005, max: 0.1, step: 0.001 },
  { name: 'insulationThickness', label: 'Толщина теплоизоляции', unit: 'м', symbol: 'Sиз', min: 0, max: 0.5, step: 0.01 },
  { name: 'drumInclination', label: 'Угол наклона барабана', unit: '°', symbol: 'β', min: 0.5, max: 10, step: 0.5 },
  { name: 'fillFactor', label: 'Коэффициент заполнения', unit: '', symbol: 'φ', min: 0.05, max: 0.3, step: 0.01 },
];

const timeFields = [
  { name: 'dryingTime', label: 'Время сушки', unit: 'с', symbol: 'τсушки', min: 60, max: 100000, step: 60 },
  { name: 'rotationSpeed', label: 'Частота вращения барабана', unit: 'об/мин', symbol: 'n', min: 0.5, max: 15, step: 0.5 },
  { name: 'materialFeedRate', label: 'Расход материала', unit: 'кг/с', symbol: 'Gм', min: 0.01, max: 50, step: 0.01 },
];

/* ===== Helper: render fields as a grid ===== */
function renderFieldsGrid(fields) {
  return (
    <Row gutter={[20, 0]}>
      {fields.map((field) => (
        <Col xs={24} md={12} key={field.name}>
          <Form.Item
            name={field.name}
            label={
              <span className="field-label">
                {field.label}
                <Tooltip title={`Обозначение: ${field.symbol}`}>
                  <Text type="secondary" className="field-symbol">
                    ({field.symbol})
                  </Text>
                </Tooltip>
              </span>
            }
            rules={[
              { required: true, message: 'Введите значение' },
              { type: 'number', min: field.min, message: `Минимум: ${field.min}` },
              { type: 'number', max: field.max, message: `Максимум: ${field.max}` },
            ]}
          >
            <InputNumber
              style={{ width: '100%' }}
              addonAfter={field.unit || undefined}
              step={field.step || 1}
              precision={field.step && field.step < 1 ? String(field.step).split('.')[1]?.length || 2 : undefined}
              placeholder={`${field.min} — ${field.max}`}
              id={`input-drum-${field.name}`}
            />
          </Form.Item>
        </Col>
      ))}
    </Row>
  );
}

/* ===== Main Page Component ===== */
export default function DrumDryerPage() {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [results, setResults] = useState(null);
  const [messageApi, contextHolder] = message.useMessage();

  const handleSubmit = async (values) => {
    setLoading(true);
    try {
      console.log('Submitted drum dryer values:', values);
      messageApi.success('Данные отправлены на расчёт!');
      const data = await calculateDrumDryer(values);
      setResults(data);
    } catch {
      messageApi.error('Ошибка при отправке данных');
    } finally {
      setLoading(false);
    }
  };

  const handleReset = () => {
    form.setFieldsValue(defaultValues);
    messageApi.info('Значения сброшены до стандартных');
  };

  const tabItems = [
    {
      key: 'material',
      label: (
        <span>
          <ExperimentOutlined /> Материал
        </span>
      ),
      children: (
        <div className="tab-content">
          <Text type="secondary" className="tab-description">
            Характеристики высушиваемого материала: масса, влажность, плотность, теплоёмкость.
          </Text>
          {renderFieldsGrid(materialFields)}
        </div>
      ),
    },
    {
      key: 'fuel',
      label: (
        <span>
          <FireOutlined /> Топливо и воздух
        </span>
      ),
      children: (
        <div className="tab-content">
          <Text type="secondary" className="tab-description">
            Параметры сушильного агента: температуры на входе и выходе, рециркуляция.
          </Text>
          {renderFieldsGrid(fuelFields)}
        </div>
      ),
    },
    {
      key: 'geometry',
      label: (
        <span>
          <RadiusSettingOutlined /> Геометрия барабана
        </span>
      ),
      children: (
        <div className="tab-content">
          <Text type="secondary" className="tab-description">
            Размеры барабана: диаметр, длина, толщина стенки, угол наклона, коэффициент заполнения.
          </Text>
          {renderFieldsGrid(geometryFields)}
        </div>
      ),
    },
    {
      key: 'time',
      label: (
        <span>
          <ClockCircleOutlined /> Режим работы
        </span>
      ),
      children: (
        <div className="tab-content">
          <Text type="secondary" className="tab-description">
            Временные и режимные параметры: время сушки, скорость вращения, расход материала.
          </Text>
          {renderFieldsGrid(timeFields)}
        </div>
      ),
    },
  ];

  /* If results are loaded, show ResultsView */
  if (results) {
    return (
      <div className="drum-dryer-page">
        {contextHolder}

        <div className="page-header animate-fade-in-up">
          <div className="page-header-icon drum-icon">
            <ExperimentOutlined />
          </div>
          <div>
            <Title level={3} style={{ margin: 0 }}>
              Сушильный барабан
            </Title>
            <Text type="secondary">Расчёт теплового баланса · Результаты</Text>
          </div>
          <Tag color="volcano" className="page-header-tag">Расчёт выполнен</Tag>
        </div>

        <DrumDryerResultsView data={results} onBack={() => setResults(null)} />
      </div>
    );
  }

  return (
    <div className="drum-dryer-page">
      {contextHolder}

      {/* Page Header */}
      <div className="page-header animate-fade-in-up">
        <div className="page-header-icon drum-icon">
          <ExperimentOutlined />
        </div>
        <div>
          <Title level={3} style={{ margin: 0 }}>
            Сушильный барабан
          </Title>
          <Text type="secondary">Расчёт теплового баланса · Ввод исходных данных</Text>
        </div>
        <Tag color="blue" className="page-header-tag">21 параметр</Tag>
      </div>

      {/* Form */}
      <Card className="form-card animate-fade-in-up delay-1">
        <Form
          form={form}
          layout="vertical"
          initialValues={defaultValues}
          onFinish={handleSubmit}
          scrollToFirstError
          requiredMark="optional"
          id="drum-dryer-form"
        >
          <Tabs
            items={tabItems}
            type="line"
            className="form-tabs"
            size="large"
          />

          <Divider />

          <div className="form-actions">
            <Space size="middle">
              <Button
                type="primary"
                htmlType="submit"
                loading={loading}
                icon={<SendOutlined />}
                size="large"
                className="submit-btn drum-submit-btn"
                id="btn-drum-calculate"
              >
                Рассчитать тепловой баланс
              </Button>
              <Button
                onClick={handleReset}
                icon={<ReloadOutlined />}
                size="large"
                id="btn-drum-reset"
              >
                Сбросить
              </Button>
            </Space>
            <Text type="secondary" className="form-hint">
              <InfoCircleOutlined /> Все поля обязательны для заполнения
            </Text>
          </div>
        </Form>
      </Card>
    </div>
  );
}
