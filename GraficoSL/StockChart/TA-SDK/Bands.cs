using System;
using Traderdata.Client.Componente.GraficoSL.Enum; 

namespace Traderdata.Client.Componente.GraficoSL.StockChart
{
  namespace Tasdk
  {
    /// <summary>
    /// Bands
    /// </summary>
    internal class Bands
    {
      /// <summary>
      /// Bollinger bands
      /// </summary>
      /// <param name="pNav">Navigator</param>
      /// <param name="pSource">Field source</param>
      /// <param name="periods">Periods</param>
      /// <param name="StandardDeviations">Standard deviation</param>
      /// <param name="MAType">Moving Average Type</param>
      /// <returns>Recordset</returns>
      public Recordset BollingerBands(Navigator pNav, Field pSource, int periods, int StandardDeviations, EnumGeral.IndicatorType MAType)
      {
        MovingAverage MA = new MovingAverage();
        Recordset Results = null;
        int Record;

        int RecordCount = pNav.RecordCount;

        if (MAType < Constants.MA_START || MAType > Constants.MA_END)
          return null;

        if (periods < 1 || periods > RecordCount)
          return null;

        if (StandardDeviations < 0 || StandardDeviations > 100)
          return null;

        Field Field1 = new Field(RecordCount, "Bollinger Band Bottom");
        Field Field2 = new Field(RecordCount, "Bollinger Band Top");

        switch (MAType)
        {
            case EnumGeral.IndicatorType.MediaMovelSimples:
            Results = MA.SimpleMovingAverage(pNav, pSource, periods, "Bollinger Band Median");
            break;
            case EnumGeral.IndicatorType.MediaMovelExponencial:
            Results = MA.ExponentialMovingAverage(pNav, pSource, periods, "Bollinger Band Median");
            break;
            case EnumGeral.IndicatorType.MediaMovelSerieTempo:
            Results = MA.TimeSeriesMovingAverage(pNav, pSource, periods, "Bollinger Band Median");
            break;
            case EnumGeral.IndicatorType.MediaMovelTriangular:
            Results = MA.TriangularMovingAverage(pNav, pSource, periods, "Bollinger Band Median");
            break;
            case EnumGeral.IndicatorType.MediaMovelVariavel:
            Results = MA.VariableMovingAverage(pNav, pSource, periods, "Bollinger Band Median");
            break;
            case EnumGeral.IndicatorType.MediaMovelPonderada:
            Results = MA.WeightedMovingAverage(pNav, pSource, periods, "Bollinger Band Median");
            break;
            case EnumGeral.IndicatorType.VIDYA:
            Results = MA.VIDYA(pNav, pSource, periods, 0.65, "Bollinger Band Median");
            break;
        }
        if (Results == null)
          return null;

        int Start = periods + 1;
        pNav.Position = Start;

        for (Record = Start; Record < RecordCount + 1; Record++)
        {
          double Sum = 0;
          double Value = Results.ValueEx("Bollinger Band Median", pNav.Position);

          int Period;
          for (Period = 1; Period < periods + 1; Period++)
          {
            Sum += (pSource.Value(pNav.Position).Value - Value) *
                   (pSource.Value(pNav.Position).Value - Value);
            pNav.MovePrevious();
          }//Period

          pNav.Position = pNav.Position + periods;

          Value = StandardDeviations * Math.Sqrt(Sum / periods);
          Field1.Value(pNav.Position,
          Results.Value("Bollinger Band Median", pNav.Position) - Value);
          Field2.Value(pNav.Position,
          Results.Value("Bollinger Band Median", pNav.Position) + Value);

          pNav.MoveNext();

        }//Record

        //Append fields to recordset
        Results.AddField(Field1);
        Results.AddField(Field2);
        return Results;
      }

      ///<summary>
      /// Moving Average Enveloper
      ///</summary>
      ///<param name="pNav">Navigator</param>
      ///<param name="pSource">Field Source</param>
      ///<param name="periods">Periods</param>
      ///<param name="MAType">Moving Average Type</param>
      ///<param name="Shift">Shift</param>
      ///<returns>Recordset</returns>
      public Recordset MovingAverageEnvelope(Navigator pNav, Field pSource, int periods, EnumGeral.IndicatorType MAType, double Shift)
      {
        MovingAverage MA = new MovingAverage();
        Recordset Results = null;
        int Record;

        int RecordCount = pNav.RecordCount;

        if (MAType < Constants.MA_START || MAType > Constants.MA_END)
          return null;

        if (periods < 1 || periods > RecordCount)
          return null;

        if (Shift < 0 || Shift > 100)
          return null;

        Field Field1 = new Field(RecordCount, "Envelope Top");
        Field Field2 = new Field(RecordCount, "Envelope Bottom");

        switch (MAType)
        {
            case EnumGeral.IndicatorType.MediaMovelSimples:
            Results = MA.SimpleMovingAverage(pNav, pSource, periods, "Temp");
            break;
            case EnumGeral.IndicatorType.MediaMovelExponencial:
            Results = MA.ExponentialMovingAverage(pNav, pSource, periods, "Temp");
            break;
            case EnumGeral.IndicatorType.MediaMovelSerieTempo:
            Results = MA.TimeSeriesMovingAverage(pNav, pSource, periods, "Temp");
            break;
            case EnumGeral.IndicatorType.MediaMovelTriangular:
            Results = MA.TriangularMovingAverage(pNav, pSource, periods, "Temp");
            break;
            case EnumGeral.IndicatorType.MediaMovelVariavel:
            Results = MA.VariableMovingAverage(pNav, pSource, periods, "Temp");
            break;
            case EnumGeral.IndicatorType.MediaMovelPonderada:
            Results = MA.WeightedMovingAverage(pNav, pSource, periods, "Temp");
            break;
            case EnumGeral.IndicatorType.VIDYA:
            Results = MA.VIDYA(pNav, pSource, periods, 0.65, "Temp");
            break;
        }
        if (Results == null)
          return null;

        pNav.MoveFirst();
        Shift = Shift / 100;

        for (Record = 1; Record < RecordCount + 1; Record++)
        {
          double Value = Results.ValueEx("Temp", pNav.Position);
          Field1.Value(pNav.Position, Value + (Value * Shift));

          Value = Results.ValueEx("Temp", pNav.Position);
          Field2.Value(pNav.Position, Value - (Value * Shift));

          pNav.MoveNext();
        }//Record

        //Append fields to recordset
        Results.AddField(Field1);
        Results.AddField(Field2);

        return Results;
      }

      ///<summary>
      /// High Low Bands
      ///</summary>
      ///<param name="pNav">Navigator</param>
      ///<param name="HighPrice">Field High Price</param>
      ///<param name="LowPrice">Field Low Price</param>
      ///<param name="ClosePrice">Field ClosePrice</param>
      ///<param name="periods">Periods</param>
      ///<returns>Recordset</returns>
      public Recordset HighLowBands(Navigator pNav, Field HighPrice, Field LowPrice, Field ClosePrice, int periods)
      {
        MovingAverage MA = new MovingAverage();
        Recordset Results = new Recordset();

        if (periods < 6 || periods > pNav.RecordCount)
          return null;

        Recordset RS1 = MA.VIDYA(pNav, HighPrice, periods, 0.8, "Maximo Minimo Bands Top");
        Recordset RS2 = MA.VIDYA(pNav, ClosePrice, periods / 2, 0.8, "Maximo Minimo Bands Median");
        Recordset RS3 = MA.VIDYA(pNav, LowPrice, periods, 0.8, "Maximo Minimo Bands Bottom");

        Results.AddField(RS1.GetField("Maximo Minimo Bands Top"));
        Results.AddField(RS2.GetField("Maximo Minimo Bands Median"));
        Results.AddField(RS3.GetField("Maximo Minimo Bands Bottom"));

        // Remove fields so recordset can be deleted
        RS1.RemoveField("Maximo Minimo Bands Top");
        RS2.RemoveField("Maximo Minimo Bands Median");
        RS3.RemoveField("Maximo Minimo Bands Bottom");

        pNav.MoveFirst();
        return Results;
      }

      ///<summary>
      /// Fractal Chaos bands
      ///</summary>
      ///<param name="pNav">Navigator</param>
      ///<param name="pOHLCV">OHLCV Recordset</param>
      ///<param name="periods">Periods</param>
      ///<returns>Records</returns>
      public Recordset FractalChaosBands(Navigator pNav, Recordset pOHLCV, int periods)
      {
        MovingAverage MA = new MovingAverage();
        Recordset Results = new Recordset();

        int RecordCount = pNav.RecordCount;
        int Record;

        if (periods < 1)
          periods = 100;

        Field fHiFractal = new Field(RecordCount, "Fractal Maximo");
        Field fLoFractal = new Field(RecordCount, "Minimo Maximo");
        Field fH = pOHLCV.GetField("Maximo");
        Field fL = pOHLCV.GetField("Minimo");
        Field fFR = new Field(RecordCount, "FR");

        Field fH1 = new Field(RecordCount, "Maximo 1");
        Field fH2 = new Field(RecordCount, "Maximo 2");
        Field fH3 = new Field(RecordCount, "Maximo 3");
        Field fH4 = new Field(RecordCount, "Maximo 4");

        Field fL1 = new Field(RecordCount, "Minimo 1");
        Field fL2 = new Field(RecordCount, "Minimo 2");
        Field fL3 = new Field(RecordCount, "Minimo 3");
        Field fL4 = new Field(RecordCount, "Minimo 4");

        for (Record = 5; Record < RecordCount + 1; ++Record)
        {
          fH1.Value(Record, fH.ValueEx(Record - 4));
          fL1.Value(Record, fL.ValueEx(Record - 4));

          fH2.Value(Record, fH.ValueEx(Record - 3));
          fL2.Value(Record, fL.ValueEx(Record - 3));

          fH3.Value(Record, fH.ValueEx(Record - 2));
          fL3.Value(Record, fL.ValueEx(Record - 2));

          fH4.Value(Record, fH.ValueEx(Record - 1));
          fL4.Value(Record, fL.ValueEx(Record - 1));
        }

        for (Record = 1; Record < RecordCount + 1; ++Record)
          fHiFractal.Value(Record, (fH.ValueEx(Record) + fL.ValueEx(Record)) / 3);

        Recordset rsFractals = MA.SimpleMovingAverage(pNav, fHiFractal, periods, "Fractal Maximo");
        fHiFractal = rsFractals.GetField("Fractal Maximo");
        rsFractals.RemoveField("Fractal Maximo");

        rsFractals = MA.SimpleMovingAverage(pNav, fLoFractal, periods, "Fractal Minimo");
        fLoFractal = rsFractals.GetField("Fractal Minimo");
        rsFractals.RemoveField("Fractal Minimo");

        for (Record = 1; Record < RecordCount + 1; ++Record)
        {
          fHiFractal.Value(Record, fH3.ValueEx(Record) + fHiFractal.ValueEx(Record));
          fLoFractal.Value(Record, fL3.ValueEx(Record) - fLoFractal.ValueEx(Record));
        }

        for (Record = 2; Record < RecordCount + 1; ++Record)
        {

          if ((fH3.Value(Record) > fH1.Value(Record)) &&
              (fH3.Value(Record) > fH2.Value(Record)) &&
              (fH3.Value(Record) >= fH4.Value(Record)) &&
              (fH3.Value(Record) >= fH.Value(Record)))
          {
            fFR.Value(Record, fHiFractal.Value(Record).Value);
          }
          else
          {
            fFR.Value(Record, 0);
          }

          if (fFR.Value(Record) == 0)
          {
            if ((fL3.Value(Record) < fL1.Value(Record)) &&
                (fL3.Value(Record) < fL2.Value(Record)) &&
                (fL3.Value(Record) <= fL4.Value(Record)) &&
                (fL3.Value(Record) <= fL.Value(Record)))
            {
              fFR.Value(Record, fLoFractal.Value(Record));
            }
            else
            {
              fFR.Value(Record, 0);
            }
          }

          if (fHiFractal.Value(Record) == fFR.Value(Record))
          {
            fHiFractal.Value(Record, fH3.Value(Record));
          }
          else
          {
            fHiFractal.Value(Record, fHiFractal.Value(Record - 1));
          }

          if (fLoFractal.Value(Record) == fFR.Value(Record))
          {
            fLoFractal.Value(Record, fL3.Value(Record));
          }
          else
          {
            fLoFractal.Value(Record, fLoFractal.Value(Record - 1));
          }

        }

        // Added 12/19/2005 TW
        for (Record = 2; Record < RecordCount + 1; ++Record)
        {
          if (fLoFractal.Value(Record) == 0)
            fLoFractal.Value(Record, null);
          if (fHiFractal.Value(Record) == 0)
            fHiFractal.Value(Record, null);
        }

        Results.AddField(fHiFractal);
        Results.AddField(fLoFractal);

        return Results;
      }

      ///<summary>
      /// Prime Number Bands
      ///</summary>
      ///<param name="pNav">Navigator</param>
      ///<param name="HighPrice">Field High Price</param>
      ///<param name="LowPrice">Field Low Price</param>
      ///<returns>Recordset</returns>
      public Recordset PrimeNumberBands(Navigator pNav, Field HighPrice, Field LowPrice)
      {
        Recordset Results = new Recordset();
        int RecordCount = pNav.RecordCount;
        int Record;
        Field fTop = new Field(RecordCount, "Prime Bands Top");
        Field fBottom = new Field(RecordCount, "Prime Bands Bottom");
        General GN = new General();
        long Top = 0, Bottom = 0;

        for (Record = 1; Record != RecordCount + 1; ++Record)
        {
          long Value = (long)(LowPrice.Value(Record));
          if (Value < 10)
            Value = Value * 10;

          long N;
          for (N = Value; N != 1; --N)
          {
            if (General.IsPrime(N))
            {
              Bottom = N;
              break;
            }
          }
          fBottom.Value(Record, Bottom);

          Value = (long)(HighPrice.Value(Record));
          if (Value < 10)
            Value = Value * 10;

          for (N = Value; N != Value * 2; ++N)
          {
            if (General.IsPrime(N))
            {
              Top = N;
              break;
            }
          }
          fTop.Value(Record, Top);
        } // Record
        Results.AddField(fTop);
        Results.AddField(fBottom);

        return Results;
      }
    }
  }
}
