import { Typography } from 'antd';
import './HeatBalanceChart.css';

const { Text } = Typography;

/**
 * Горизонтальная столбчатая диаграмма распределения теплоты.
 * Без тяжёлых библиотек — CSS-bars.
 */

const COLORS = [
  '#e8590c', // Q1 - оранжевый
  '#fa8c16', // Q2 - золотой
  '#faad14', // Q3 - жёлтый
  '#13c2c2', // Q5 топ - бирюзовый
  '#1890ff', // Q5 р.п - синий
  '#722ed1', // Q6 - фиолетовый
];

export default function HeatBalanceChart({ items, title }) {
  if (!items || items.length === 0) return null;

  const maxValue = Math.max(...items.map((i) => i.value));

  return (
    <div className="hb-chart">
      {title && (
        <Text strong className="hb-chart-title">{title}</Text>
      )}
      <div className="hb-chart-bars">
        {items.map((item, index) => {
          const widthPercent = (item.value / maxValue) * 100;
          const color = COLORS[index % COLORS.length];

          return (
            <div className="hb-bar-row" key={item.key}>
              <div className="hb-bar-label">
                <span
                  className="hb-bar-dot"
                  style={{ background: color }}
                />
                <Text className="hb-bar-name" ellipsis={{ tooltip: true }}>
                  {item.label.replace(/\(.+\)/, '').trim()}
                </Text>
              </div>
              <div className="hb-bar-track">
                <div
                  className="hb-bar-fill"
                  style={{
                    width: `${widthPercent}%`,
                    background: `linear-gradient(90deg, ${color} 0%, ${color}cc 100%)`,
                    animationDelay: `${index * 0.1}s`,
                  }}
                />
              </div>
              <div className="hb-bar-values">
                <Text className="hb-bar-percent">{item.percent.toFixed(1)}%</Text>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
