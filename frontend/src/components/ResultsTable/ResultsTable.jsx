import { Table, Tag, Typography } from 'antd';
import './ResultsTable.css';

const { Text } = Typography;

/**
 * Универсальная таблица результатов теплового баланса.
 * @param {{ items: Array<{key, label, value, percent, unit}>, totalHeat: {value, unit, label} }} props
 */
export default function ResultsTable({ items, totalHeat }) {
  const columns = [
    {
      title: 'Статья баланса',
      dataIndex: 'label',
      key: 'label',
      render: (text) => <Text strong={false}>{text}</Text>,
    },
    {
      title: 'Значение',
      dataIndex: 'value',
      key: 'value',
      width: 180,
      align: 'right',
      render: (val, record) => (
        <span className="results-value">
          {formatNumber(val)}
          <Text type="secondary" className="results-unit"> {record.unit}</Text>
        </span>
      ),
    },
    {
      title: 'Доля, %',
      dataIndex: 'percent',
      key: 'percent',
      width: 140,
      align: 'center',
      render: (val) => (
        <div className="percent-cell">
          <div className="percent-bar-bg">
            <div
              className="percent-bar-fill"
              style={{ width: `${Math.min(val, 100)}%` }}
            />
          </div>
          <Text className="percent-value">{val.toFixed(2)}%</Text>
        </div>
      ),
    },
  ];

  const summaryRow = () => (
    <Table.Summary fixed>
      <Table.Summary.Row className="summary-row">
        <Table.Summary.Cell index={0}>
          <Text strong>{totalHeat.label}</Text>
        </Table.Summary.Cell>
        <Table.Summary.Cell index={1} align="right">
          <Text strong className="results-value summary-value">
            {formatNumber(totalHeat.value)}
            <Text type="secondary" className="results-unit"> {totalHeat.unit}</Text>
          </Text>
        </Table.Summary.Cell>
        <Table.Summary.Cell index={2} align="center">
          <Tag color="volcano" className="total-tag">100%</Tag>
        </Table.Summary.Cell>
      </Table.Summary.Row>
    </Table.Summary>
  );

  return (
    <Table
      columns={columns}
      dataSource={items}
      pagination={false}
      bordered
      size="middle"
      className="results-table"
      summary={summaryRow}
      rowKey="key"
    />
  );
}

/** Форматирование числа с разделителями тысяч и 2 знаками */
function formatNumber(num) {
  if (num == null) return '—';
  return num.toLocaleString('ru-RU', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });
}
