/**
 * Мок-данные ответа от Java-микросервиса камерной сушильной печи.
 * Структура основана на Excel файле «Тепловой баланс камерной сушильной печи».
 */

export const chamberFurnaceMockResponse = {
  /* ===== Расчет горения топлива ===== */
  fuelCombustion: {
    // Аэродинамический и температурный режимы
    aerodynamics: {
      relativeConsumption: { value: 3.763, unit: '', symbol: 'Z', label: 'Относительный расход' },
      exitEnthalpy: { value: 711.45, unit: 'кДж/м³', symbol: 'i₀', label: 'Энтальпия газов на выходе из топки' },
      exitTemperature: { value: 508.18, unit: '°С', symbol: 't₀', label: 'Температура газов на выходе из топки' },
      flueGasEnthalpy: { value: 2969.48, unit: 'кДж/м³', symbol: 'Iф', label: 'Теплосодержание топочных газов' },
      airAmountToReduce: { value: 3.29, unit: 'м³/м³', symbol: 'Хв', label: 'Кол-во воздуха для снижения энтальпии' },
    },

    // Расчет O2 и воздуха
    oxygenAndAir: {
      oxygenVolume: { value: 2.074, unit: 'м³/м³', symbol: 'Vo₂', label: 'Объём кислорода' },
      airVolumeTheoretical: { value: 9.872, unit: 'м³/м³', symbol: 'Lo', label: 'Теоретический объём воздуха' },
      airVolumeActual: { value: 11.352, unit: 'м³/м³', symbol: 'Lα', label: 'Действительный объём воздуха' },
    },

    // Продукты сгорания при α=1
    combustionProductsAlpha1: {
      co2: { value: 1.067, unit: 'м³/м³', symbol: 'V°CO₂', label: 'CO₂' },
      h2o: { value: 2.034, unit: 'м³/м³', symbol: 'V°H₂O', label: 'H₂O' },
      n2: { value: 7.827, unit: 'м³/м³', symbol: 'V°N₂', label: 'N₂' },
      total: { value: 10.929, unit: 'м³/м³', symbol: 'V°', label: 'Общий объём' },
    },

    // Продукты сгорания при α=1.15
    combustionProductsAlpha115: {
      co2: { value: 1.067, unit: 'м³/м³', symbol: 'VαCO₂', label: 'CO₂' },
      h2o: { value: 2.060, unit: 'м³/м³', symbol: 'VαH₂O', label: 'H₂O' },
      n2: { value: 8.997, unit: 'м³/м³', symbol: 'VαN₂', label: 'N₂' },
      o2excess: { value: 0.311, unit: 'м³/м³', symbol: 'VО₂изб', label: 'O₂ избыточный' },
      total: { value: 12.435, unit: 'м³/м³', symbol: 'Vα', label: 'Общий объём' },
    },

    // Состав при α=1 (%)
    compositionAlpha1: {
      co2: { value: 9.77, unit: '%', symbol: 'V°CO₂', label: 'CO₂' },
      h2o: { value: 18.61, unit: '%', symbol: 'V°H₂O', label: 'H₂O' },
      n2: { value: 71.62, unit: '%', symbol: 'V°N₂', label: 'N₂' },
    },

    // Состав при α=1.15 (%)
    compositionAlpha115: {
      co2: { value: 8.58, unit: '%', symbol: 'VαCO₂', label: 'CO₂' },
      h2o: { value: 16.56, unit: '%', symbol: 'VαH₂O', label: 'H₂O' },
      n2: { value: 72.35, unit: '%', symbol: 'VαN₂', label: 'N₂' },
      o2excess: { value: 2.50, unit: '%', symbol: 'VО₂изб', label: 'O₂ избыточный' },
    },

    // Теплота сгорания
    combustionHeat: {
      lowerHeatValue: { value: 37222.40, unit: 'кДж/м³', symbol: 'Qₙ', label: 'Низшая теплота сгорания' },
      theoreticalCombustionTemp: { value: 1848.34, unit: '°С', symbol: 'tтα', label: 'Теоретическая температура горения' },
      balanceCombustionTemp: { value: 1825.53, unit: '°С', symbol: 'tбα', label: 'Балансовая температура горения' },
    },
  },

  /* ===== Тепловой баланс (без рециркуляции) ===== */
  heatBalanceNoRecirculation: {
    items: [
      { key: 'Q1', label: 'Расход теплоты на нагрев (Q₁)', value: 4530545.04, percent: 13.78, unit: 'кДж' },
      { key: 'Q2', label: 'Потери теплоты с отходящими газами (Q₂)', value: 8267902.04, percent: 25.15, unit: 'кДж' },
      { key: 'Q3', label: 'Потери вследствие хим. недожога (Q₃)', value: 657527.79, percent: 2.00, unit: 'кДж' },
      { key: 'Q5_furnace', label: 'Потери теплоты топкой (Q₅ топ)', value: 6575277.90, percent: 20.00, unit: 'кДж' },
      { key: 'Q5_walls', label: 'Потери на нагрев стенок (Q₅ р.п)', value: 10937971.99, percent: 33.27, unit: 'кДж' },
      { key: 'Q6', label: 'Аккумулированное тепло (Q₆)', value: 1907171.75, percent: 5.80, unit: 'кДж' },
    ],
    totalHeat: { value: 32876396.52, unit: 'кДж', label: 'Сумма Q' },
  },

  /* ===== Тепловой баланс (с рециркуляцией) ===== */
  heatBalanceWithRecirculation: {
    items: [
      { key: 'Q1p', label: 'Расход теплоты на нагрев (Q₁\')', value: 4530545.04, percent: 13.78, unit: 'кДж' },
      { key: 'Q2p', label: 'Потери теплоты с отходящими газами (Q₂\')', value: 8267902.04, percent: 25.15, unit: 'кДж' },
      { key: 'Q3p', label: 'Потери вследствие хим. недожога (Q₃\')', value: 657527.79, percent: 2.00, unit: 'кДж' },
      { key: 'Q5_furnace_p', label: 'Потери теплоты топкой (Q₅\' топ)', value: 6575277.90, percent: 20.00, unit: 'кДж' },
      { key: 'Q5_walls_p', label: 'Потери на нагрев стенок (Q₅\' р.п)', value: 10937971.99, percent: 33.27, unit: 'кДж' },
      { key: 'Q6p', label: 'Аккумулированное тепло (Q₆\')', value: 1907171.75, percent: 5.80, unit: 'кДж' },
    ],
    totalHeat: { value: 32876396.52, unit: 'кДж', label: 'Сумма Q\'' },
  },

  /* ===== Ключевые показатели ===== */
  summary: {
    efficiency: { value: 13.78, unit: '%', symbol: 'η', label: 'КПД печи' },
    fuelConsumption: { value: 0.0307, unit: 'м³/с', symbol: 'В', label: 'Расход топлива' },
    totalFuelHeat: { value: 32876396.52, unit: 'кДж', symbol: 'Qxh', label: 'Теплота топлива' },
    heatPerKgMoisture: { value: 49402.27, unit: 'кДж/кг', symbol: 'qисп', label: 'Расход теплоты на 1 кг влаги' },
    removedMoisture: { value: 665.48, unit: 'кг', symbol: 'Gвод', label: 'Количество удалённой влаги' },
  },
};

/**
 * Имитация запроса к API с задержкой.
 * @param {object} _inputData - данные формы (не используются в моке)
 * @returns {Promise<object>} мок-ответ
 */
export function fetchChamberFurnaceResult(_inputData) {
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve(chamberFurnaceMockResponse);
    }, 1200 + Math.random() * 800);
  });
}
