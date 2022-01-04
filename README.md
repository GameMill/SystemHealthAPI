# System Health API

This is a System Health Checker that exposes a lot of system information to an online endpoint

## How to use
Run the program and wait for the console to disappear.

Then your can access the information locally by clicking on the tooltip to show the window and then right click on the tooltip again and click print info.

To view online you can access the API via http://{IP-Address}:10000/?callback={JSONP-Callback-Name}


## example response with JSONP callback (rf21f1)
rf21f1({"FirstHddRun":{"D:\\":true,"E:\\":true,"F:\\":true,"G:\\":true,"C:\\":true},"CPU":{"Cores":[{"Name":"CPU Core #1","Temperature":0.0,"Current":54.63918,"Min":9.375,"Max":65.46392},{"Name":"CPU Core #2","Temperature":0.0,"Current":62.8865967,"Min":39.0625,"Max":63.0208359},{"Name":"CPU Core #3","Temperature":0.0,"Current":55.15464,"Min":23.7704048,"Max":58.76289},{"Name":"CPU Core #4","Temperature":0.0,"Current":22.1649532,"Min":3.09278369,"Max":66.14583},{"Name":"CPU Core #5","Temperature":0.0,"Current":38.6597977,"Min":4.16666269,"Max":71.3541641},{"Name":"CPU Core #6","Temperature":0.0,"Current":13.4020624,"Min":2.08333135,"Max":49.48454},{"Name":"CPU Core #7","Temperature":0.0,"Current":3.09278369,"Min":1.55175924,"Max":6.25},{"Name":"CPU Core #8","Temperature":0.0,"Current":8.762884,"Min":2.60416269,"Max":40.1041641},{"Name":"CPU Core #9","Temperature":0.0,"Current":5.15463924,"Min":2.06185579,"Max":19.7916622},{"Name":"CPU Core #10","Temperature":0.0,"Current":12.3711348,"Min":9.375,"Max":24.5535736},{"Name":"CPU Core #11","Temperature":0.0,"Current":1.54639482,"Min":1.54639482,"Max":9.36426},{"Name":"CPU Core #12","Temperature":0.0,"Current":2.06185579,"Min":1.5625,"Max":9.79381752}],"Total":{"Name":"Total","Temperature":0.0,"Current":23.3247452,"Min":50.625,"Max":60.125},"Name":"AMD Ryzen 9 3900X"},"Gpu":{"Temperature":{"Name":"Total","Temperature":0.0,"Current":33.0,"Min":33.0,"Max":33.0},"Usage":{"Name":"Usage","Temperature":0.0,"Current":23.0,"Min":23.0,"Max":46.0},"TotalMemory":8192,"MemoryUsed":{"Name":"MemoryUsage","Temperature":0.0,"Current":1735.66406,"Min":1713.07031,"Max":1744.25781},"Fan":{"Name":"FanUsage","Temperature":0.0,"Current":0.0,"Min":0.0,"Max":0.0},"Name":"NVIDIA NVIDIA GeForce RTX 3070"},"ram":{"Load":{"Name":null,"Temperature":0.0,"Current":46.8729935,"Min":46.85925,"Max":46.894516},"UsedMemory":{"Name":null,"Temperature":0.0,"Current":14.9632,"Min":14.9588127,"Max":14.97007},"AvailableMemory":{"Name":null,"Temperature":0.0,"Current":16.95966,"Min":16.95279,"Max":16.9640465},"Name":null},"Drives":[{"Label":"New Volume","TotalSize":7452.019527435302734375,"TotalFreeSize":5187.0019683837890625,"UsedProcentage":30.3946819,"Mount":"D:\\","Temperature":28.0,"Name":"WDC WD82PURZ-85TEUY0"},{"Label":"OLD OS","TotalSize":953.583980560302734375,"TotalFreeSize":379.807682037353515625,"UsedProcentage":60.1705055,"Mount":"E:\\","Temperature":40.0,"Name":"AS25 1TB"},{"Label":"500GB HD","TotalSize":476.813472747802734375,"TotalFreeSize":2.7058258056640625,"UsedProcentage":99.43252,"Mount":"F:\\","Temperature":17.0,"Name":"SanDisk SD8SB8U512G1122"},{"Label":"Fast 1tb (M.2)","TotalSize":931.496089935302734375,"TotalFreeSize":400.555706024169921875,"UsedProcentage":56.9986725,"Mount":"G:\\","Temperature":0.0,"Name":"Generic Hard Disk"},{"Label":"500GB M.2","TotalSize":465.476558685302734375,"TotalFreeSize":21.168209075927734375,"UsedProcentage":95.45233,"Mount":"C:\\","Temperature":0.0,"Name":"Generic Hard Disk"}],"networks":[]})
