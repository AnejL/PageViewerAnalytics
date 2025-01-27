using ViewerTelemetry.Model;
using ViewerTelemetry.Model.Contracts;

while (true)
{
    var target = new VitapurHybrid20TelemetryTarget();

    var value = await target.SendRequest();

    if (value != null)
        await target.AppendValueToFile(value);

    await Task.Delay(2000);
}