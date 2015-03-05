using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ClearScript.NodeEmulation;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClearScript.Manager;
using System.Net.Http;
using System.Threading;
using System.Net;
using System.Net.Http.Headers;

namespace ConsoleApplication2
{
    class Program
    {
        public class Foo

        {

            public class FooFactory


            {
                public Foo MakeIt
                    ()
                {
                    return new Foo();
                }
            }

            public string go()
            {
                return "hey";
            }
        }

        static object require(string thing)
        {
            return new Foo();
        }

        private static void Main(string[] args)
        {

            var hostname = "food.com";
            var port = 80;
            var path = "/abc/deasdk";

            int qpos = path.IndexOf('?');


            var pathParts = (path ?? "").Split(new char[] { '?' }, 2);
            var uriBuilder = new UriBuilder(
               "http",
              hostname,
              port,
              qpos>-1? path.Substring(0,qpos ) : path,
              qpos>-1? path.Substring (qpos) :null).Uri;




            //var managerPool = new ManagerPool(new ManagerSettings());
            var settings = new ManagerSettings()
                           {

                           };
            ManagerPool.InitializeCurrentPool(new ManualManagerSettings()
                                              {
                                                 V8DebugEnabled = true ,
                                                  V8DebugPort = 5858,
                                                  MaxExecutableBytes =0,
                                                  MaxNewSpaceBytes = 0,
                                                  MaxOldSpaceBytes =0,
                                                  RuntimeMaxCount = 10
                                                  
                                              });
            //ManagerPool.InitializeCurrentPool(new ManagerSettings()
            //                                  {

            //                                  });

            for (int i = 0; i < 8; i++)
            {
                Task t = new Task(() => main2());
                t.Start();
            }
            //main2();
            Console.ReadLine();
            Console.WriteLine("gcing");
         //   main2();
        }
        static UIntPtr heapSize = new UIntPtr(1000000000);
            
        private static int cnt = 0;
        static void main2()
        {
        //var engine = runtime.CreateScriptEngine();

           
            while (true)
            {
                using (var scope = new ManagerScope())
                {
                    var runtime = scope.RuntimeManager;

                    //V8Runtime v8Runtime = (V8Runtime)runtime.GetType().InvokeMember("_v8Runtime", BindingFlags.GetField | BindingFlags.NonPublic| BindingFlags.Instance, null, runtime,null);
                    //V8Runtime v8Runtime = new V8Runtime();
                    //runtime.GetType().InvokeMember("_v8Runtime", BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Instance, null, runtime, new object[] { v8Runtime });
                    //V8ScriptEngine engine = v8Runtime.CreateScriptEngine("steve", V8ScriptEngineFlags.DisableGlobalMembers| V8ScriptEngineFlags.None ,5858);

                    V8ScriptEngine engine = runtime.GetEngine();




                    if (engine.MaxRuntimeHeapSize != heapSize)
                    {
                        engine.MaxRuntimeHeapSize = heapSize;
                        engine.RuntimeHeapSizeSampleInterval = TimeSpan.FromSeconds(30);
                    }
                    //var heapinfo = engine.GetRuntimeHeapInfo();

                
                    //if (heapinfo.TotalHeapSize > 100000000)
                    //{
                    //    Console.WriteLine("{0} {1} {2}", heapinfo.TotalHeapSize , heapinfo.TotalHeapSizeExecutable, heapinfo.UsedHeapSize);
                    //    engine.CollectGarbage(false);
                    //    heapinfo = engine.GetRuntimeHeapInfo();
                    //    Console.WriteLine("{0} {1} {2}", heapinfo.TotalHeapSize, heapinfo.TotalHeapSizeExecutable, heapinfo.UsedHeapSize);
                    //    engine.MaxRuntimeHeapSize = new UIntPtr(90000000);
                    //}
                    try
                    {
                     //   engine.WriteHeapSnapshot("c:\\temp\\foo" + cnt + ".heapsnapshot");
                       // while (true)
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                TryIt(runtime, engine);
                            }
                           
                        }
                        
                       engine.Dispose();
                        
                    }
                    catch (ScriptEngineException see)
                    {
                        System.Diagnostics.Debug.WriteLine(see.Message);
                        System.Diagnostics.Debug.WriteLine(see.ErrorDetails);
                        int f = 0;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                        int f = 0;
                    }
                   
                }
                
                int fg = 0;
            }
        }

        public class MozuHostedEnv
        {
            private SdkConfigClass config = new SdkConfigClass();
            public SdkConfigClass sdkConfig
            {
                get
                {
                    return config;
                    
                }
            }
        }

        public class SdkConfigClass
        {
            public string baseUrl{ get { return "https://home.mozu.com/"; }}

            public string tenantPod { get { return "http://tp1.mozu.com/"; } }   
                
        }

        static void TryIt(IRuntimeManager runtime, V8ScriptEngine engine)
        {

            
            var require = new Require(runtime, engine);
            
            require.BuiltIns.process.env.mozuHosted = new MozuHostedEnv();
            require.RequestHandlerFactory = ()=>new DelegatingHandler[]{ new MyDelegatingHandler()}; 
         //   require.BuiltIns.process.env.NODE_DEBUG = "request";

            var file = new System.IO.FileInfo(Environment.CurrentDirectory + @"..\..\..\js\rateProvider.built.js");
            if (!file.Exists)
            {
                Console.WriteLine("run npm install and grunt");
                Environment.Exit(1);
            }




            var module = require.LoadModuleByPath(file.FullName);

           // var rateProvider = rateProviderFactory.getRateProvider();


            for (int i = 0; i < 100; i++)
            {
                var cb = new CallBacker();
                var pb = new PropertyBag();




                var bing = module;
                if (Sstopwatch == null)
                {
                    Sstopwatch = new Stopwatch();
                    Sstopwatch.Start();
                }

                //System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() =>
                //{
                //    bing.getRatesAsync(pb, cb.Callback);
                //});

                engine.NextTick(() =>
                {
                    bing.getRatesAsync(pb, cb.Callback);    
                });
                
                //  provider.getRatesAsync(pb, cb.Callback);
                cb.T.ConfigureAwait(false);
                cb.T.Wait();
                object joke;
                pb.TryGetValue("joke", out joke);
             
              
                System.Diagnostics.Debug.WriteLine(joke);
                cnt ++;
                if (cnt%10000==0)
                {
                    engine.WriteHeapSnapshot("c:\\temp\\foo"+cnt+".heapsnapshot");
                }
                var totsSecs = Sstopwatch.ElapsedMilliseconds == 0 ? 1 : (cnt*1000)/(Sstopwatch.ElapsedMilliseconds);
                
                Console.WriteLine("{0} a sec", totsSecs);
               // require.Reset();



              


                //engine.CollectGarbage(true);
                //GC.Collect(0, GCCollectionMode.Forced);
                //GC.Collect(2, GCCollectionMode.Forced);
                //engine.CollectGarbage(true);
                //System.Threading.Thread.Sleep(1005 * 60 * 3); 
                //System.Threading.Thread.Sleep(5000);
                //GC.Collect(0, GCCollectionMode.Forced);
                //GC.Collect(2, GCCollectionMode.Forced);
                //engine.CollectGarbage(true);
                //engine.WriteHeapSnapshot("c:\\temp\\fart.heapsnapshot");
                
            }


        }

        private static Stopwatch Sstopwatch = null;



    }

        public class MyDelegatingHandler : DelegatingHandler
        {
            private HttpMessageInvoker _server;

            public void SetHandler(HttpMessageHandler handler)
            {
                _server = new HttpMessageInvoker(handler);
            }
            protected async  override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
            {

                try
                {
                    var ticket = "{\"refreshToken\":\"7cc8c5873d98465ab25aff43932662bb\",\"accessToken\":\"OgxSpL59hCRwtl+DVGd9DdEgIvAZPFxf35v1FnVlNDPPvwvC2C9UerLAblYj4y0+zK51Z4VdMtyr+BJ9SWD+bJ74TRlP4UTj7PFF1O6CYhfb6k+iV5TKGPX4001yIBiAoKPDLF8sD8/B6do9KBhFv9z+8DGEom87sMIBqgA/q5z+faosTRFowXBiMaGvJiivZDlIVoa1V16dnS4pg6oQZwT2x+D10Pq6Z1fV3WwH0P+V0twCkGJDDAwrdKvLIY5P4yDMtHbOyo9HiMyHZEHM1MufF766CPn0sco15ivr0Mm1W7nqAMDJc2jHJiiaMB9HyYnJZOvqmxmgLXghTTszZ6aKhRo+pWcyrpCG9dEIsAptppuaYCbNSzYqYuNShV7MuMPFQd2R0q3AW4dpshLgPd4r2jHfrnt0nLQK3RJ1WXiEf1LO7YZRhHJZbWQG02gSZ+VR2ZXwyL7aTEXBF+UcpA==\",\"accessTokenExpiration\":\"2015-02-15T02:08:18.428Z\",\"refreshTokenExpiration\":\"2015-02-15T13:08:18.428Z\"}";
                    String product = "{\"productCode\":\"MS-CAR-RAK-006\",\"productUsage\":\"Configurable\",\"fulfillmentTypesSupported\":[\"DirectShip\",\"InStorePickup\"],\"masterCatalogId\":1,\"productSequence\":532,\"productTypeId\":8,\"isValidForProductType\":true,\"productInCatalogs\":[{\"catalogId\":1,\"isActive\":true,\"isContentOverridden\":false,\"content\":{\"localeCode\":\"en-US\",\"productName\":\"Yakima Universal ForkLift Bike Rack\",\"productFullDescription\":\"<div><font face=\\\"SourceSansProRegular, helvetica, arial, verdana, sans-serif\\\" size=\\\"3\\\">A first for Yakima, a fork-mount bike rack that attaches directly to most factory crossbars right out of the box! Meet the the Yakima Universal ForkLift.</font></div><div><font face=\\\"SourceSansProRegular, helvetica, arial, verdana, sans-serif\\\" size=\\\"3\\\"><br></font></div><div><ul><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Rack installs quickly and easily, and it fits round, square and most factory crossbars</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Rack is designed to accommodate most bike forks with disc brakes</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Sliding wheel tray makes positioning the rear wheel simple</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Lockable skewer has an integrated adjustment knob for simple 1-handed operation; SKS lock cores are sold separately</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Sleek design and nice finish look great on your vehicle</span><br></li></ul></div>\",\"productShortDescription\":\"<div><font face=\\\"SourceSansProRegular, helvetica, arial, verdana, sans-serif\\\" size=\\\"3\\\">A first for Yakima, a fork-mount bike rack that attaches directly to most factory crossbars right out of the box! Meet the the Yakima Universal ForkLift.</font></div>\",\"productImages\":[{\"id\":6621,\"localeCode\":\"en-US\",\"cmsId\":\"9fe889dc-e9a7-4cc3-a1c0-61c24b105d71\",\"sequence\":1},{\"id\":6622,\"localeCode\":\"en-US\",\"cmsId\":\"79b12823-8e32-4a73-8101-aae4096aaeda\",\"sequence\":2},{\"id\":6623,\"localeCode\":\"en-US\",\"cmsId\":\"2aaa72df-3a40-43c9-a4d8-27df89242901\",\"sequence\":3}]},\"isPriceOverridden\":false,\"price\":{\"isoCurrencyCode\":\"USD\",\"price\":159.0000,\"msrp\":174.9000},\"isSeoContentOverridden\":true,\"seoContent\":{\"localeCode\":\"en-US\",\"metaTagTitle\":\"Yakima Universal ForkLift Bike Rack \",\"metaTagDescription\":\"A first for Yakima, a fork-mount bike rack that attaches directly to most factory crossbars right out of the box! Meet the the Yakima Universal ForkLift.\",\"metaTagKeywords\":\"\",\"seoFriendlyUrl\":\"yakima-universal-forklift-bike-rack-\"},\"productCategories\":[{\"categoryId\":1},{\"categoryId\":5}],\"auditInfo\":{\"updateDate\":\"2014-06-04T22:23:28.495Z\",\"createDate\":\"2013-12-31T21:11:28.611Z\",\"updateBy\":\"dc688cbabd7a4672a95da25c00bafde9\",\"createBy\":\"538f588d632e4533970da29a00d26200\"}},{\"catalogId\":2,\"isActive\":true,\"isContentOverridden\":false,\"content\":{\"localeCode\":\"en-US\",\"productName\":\"Yakima Universal ForkLift Bike Rack\",\"productFullDescription\":\"<div><font face=\\\"SourceSansProRegular, helvetica, arial, verdana, sans-serif\\\" size=\\\"3\\\">A first for Yakima, a fork-mount bike rack that attaches directly to most factory crossbars right out of the box! Meet the the Yakima Universal ForkLift.</font></div><div><font face=\\\"SourceSansProRegular, helvetica, arial, verdana, sans-serif\\\" size=\\\"3\\\"><br></font></div><div><ul><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Rack installs quickly and easily, and it fits round, square and most factory crossbars</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Rack is designed to accommodate most bike forks with disc brakes</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Sliding wheel tray makes positioning the rear wheel simple</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Lockable skewer has an integrated adjustment knob for simple 1-handed operation; SKS lock cores are sold separately</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Sleek design and nice finish look great on your vehicle</span><br></li></ul></div>\",\"productShortDescription\":\"<div><font face=\\\"SourceSansProRegular, helvetica, arial, verdana, sans-serif\\\" size=\\\"3\\\">A first for Yakima, a fork-mount bike rack that attaches directly to most factory crossbars right out of the box! Meet the the Yakima Universal ForkLift.</font></div>\",\"productImages\":[{\"id\":6621,\"localeCode\":\"en-US\",\"cmsId\":\"9fe889dc-e9a7-4cc3-a1c0-61c24b105d71\",\"sequence\":1},{\"id\":6622,\"localeCode\":\"en-US\",\"cmsId\":\"79b12823-8e32-4a73-8101-aae4096aaeda\",\"sequence\":2},{\"id\":6623,\"localeCode\":\"en-US\",\"cmsId\":\"2aaa72df-3a40-43c9-a4d8-27df89242901\",\"sequence\":3}]},\"isPriceOverridden\":false,\"price\":{\"isoCurrencyCode\":\"USD\",\"price\":159.0000,\"msrp\":174.9000},\"isSeoContentOverridden\":true,\"seoContent\":{\"localeCode\":\"en-US\",\"metaTagTitle\":\"Yakima Universal ForkLift Bike Rack \",\"metaTagDescription\":\"A first for Yakima, a fork-mount bike rack that attaches directly to most factory crossbars right out of the box! Meet the the Yakima Universal ForkLift.\",\"metaTagKeywords\":\"\",\"seoFriendlyUrl\":\"yakima-universal-forklift-bike-rack-\"},\"productCategories\":[{\"categoryId\":37},{\"categoryId\":43}],\"auditInfo\":{\"updateDate\":\"2014-06-04T22:23:28.495Z\",\"createDate\":\"2013-12-31T21:11:28.611Z\",\"updateBy\":\"dc688cbabd7a4672a95da25c00bafde9\",\"createBy\":\"538f588d632e4533970da29a00d26200\"}},{\"catalogId\":3,\"isActive\":true,\"isContentOverridden\":false,\"content\":{\"localeCode\":\"en-US\",\"productName\":\"Yakima Universal ForkLift Bike Rack\",\"productFullDescription\":\"<div><font face=\\\"SourceSansProRegular, helvetica, arial, verdana, sans-serif\\\" size=\\\"3\\\">A first for Yakima, a fork-mount bike rack that attaches directly to most factory crossbars right out of the box! Meet the the Yakima Universal ForkLift.</font></div><div><font face=\\\"SourceSansProRegular, helvetica, arial, verdana, sans-serif\\\" size=\\\"3\\\"><br></font></div><div><ul><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Rack installs quickly and easily, and it fits round, square and most factory crossbars</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Rack is designed to accommodate most bike forks with disc brakes</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Sliding wheel tray makes positioning the rear wheel simple</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Lockable skewer has an integrated adjustment knob for simple 1-handed operation; SKS lock cores are sold separately</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Sleek design and nice finish look great on your vehicle</span><br></li></ul></div>\",\"productShortDescription\":\"<div><font face=\\\"SourceSansProRegular, helvetica, arial, verdana, sans-serif\\\" size=\\\"3\\\">A first for Yakima, a fork-mount bike rack that attaches directly to most factory crossbars right out of the box! Meet the the Yakima Universal ForkLift.</font></div>\",\"productImages\":[{\"id\":6621,\"localeCode\":\"en-US\",\"cmsId\":\"9fe889dc-e9a7-4cc3-a1c0-61c24b105d71\",\"sequence\":1},{\"id\":6622,\"localeCode\":\"en-US\",\"cmsId\":\"79b12823-8e32-4a73-8101-aae4096aaeda\",\"sequence\":2},{\"id\":6623,\"localeCode\":\"en-US\",\"cmsId\":\"2aaa72df-3a40-43c9-a4d8-27df89242901\",\"sequence\":3}]},\"isPriceOverridden\":false,\"price\":{\"isoCurrencyCode\":\"USD\",\"price\":159.0000,\"msrp\":174.9000},\"isSeoContentOverridden\":true,\"seoContent\":{\"localeCode\":\"en-US\",\"metaTagTitle\":\"Yakima Universal ForkLift Bike Rack \",\"metaTagDescription\":\"A first for Yakima, a fork-mount bike rack that attaches directly to most factory crossbars right out of the box! Meet the the Yakima Universal ForkLift.\",\"metaTagKeywords\":\"\",\"seoFriendlyUrl\":\"yakima-universal-forklift-bike-rack-\"},\"productCategories\":[{\"categoryId\":29},{\"categoryId\":42}],\"auditInfo\":{\"updateDate\":\"2014-06-04T22:23:28.495Z\",\"createDate\":\"2013-12-31T21:11:28.611Z\",\"updateBy\":\"dc688cbabd7a4672a95da25c00bafde9\",\"createBy\":\"538f588d632e4533970da29a00d26200\"}}],\"content\":{\"localeCode\":\"en-US\",\"productName\":\"Yakima Universal ForkLift Bike Rack\",\"productFullDescription\":\"<div><font face=\\\"SourceSansProRegular, helvetica, arial, verdana, sans-serif\\\" size=\\\"3\\\">A first for Yakima, a fork-mount bike rack that attaches directly to most factory crossbars right out of the box! Meet the the Yakima Universal ForkLift.</font></div><div><font face=\\\"SourceSansProRegular, helvetica, arial, verdana, sans-serif\\\" size=\\\"3\\\"><br></font></div><div><ul><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Rack installs quickly and easily, and it fits round, square and most factory crossbars</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Rack is designed to accommodate most bike forks with disc brakes</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Sliding wheel tray makes positioning the rear wheel simple</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Lockable skewer has an integrated adjustment knob for simple 1-handed operation; SKS lock cores are sold separately</span><br></li><li><span style=\\\"font-family: SourceSansProRegular, helvetica, arial, verdana, sans-serif; font-size: medium;\\\">Sleek design and nice finish look great on your vehicle</span><br></li></ul></div>\",\"productShortDescription\":\"<div><font face=\\\"SourceSansProRegular, helvetica, arial, verdana, sans-serif\\\" size=\\\"3\\\">A first for Yakima, a fork-mount bike rack that attaches directly to most factory crossbars right out of the box! Meet the the Yakima Universal ForkLift.</font></div>\",\"productImages\":[{\"id\":6621,\"localeCode\":\"en-US\",\"cmsId\":\"9fe889dc-e9a7-4cc3-a1c0-61c24b105d71\",\"sequence\":1},{\"id\":6622,\"localeCode\":\"en-US\",\"cmsId\":\"79b12823-8e32-4a73-8101-aae4096aaeda\",\"sequence\":2},{\"id\":6623,\"localeCode\":\"en-US\",\"cmsId\":\"2aaa72df-3a40-43c9-a4d8-27df89242901\",\"sequence\":3}]},\"price\":{\"isoCurrencyCode\":\"USD\",\"price\":159.0000,\"msrp\":174.9000},\"pricingBehavior\":{\"discountsRestricted\":false},\"seoContent\":{\"localeCode\":\"en-US\",\"metaTagTitle\":\"Yakima Universal ForkLift Bike Rack\",\"metaTagDescription\":\"A first for Yakima, a fork-mount bike rack that attaches directly to most factory crossbars right out of the box! Meet the the Yakima Universal ForkLift.\",\"metaTagKeywords\":\"\",\"titleTagTitle\":\"Yakima Universal ForkLift Bike Rack\",\"seoFriendlyUrl\":\"yakima-universal-forklift-bike-rack-\"},\"options\":[{\"attributeFQN\":\"Tenant~Color\",\"values\":[{\"value\":\"Black\",\"attributeVocabularyValueDetail\":{\"valueSequence\":72,\"value\":\"Black\",\"content\":{\"localeCode\":\"en-US\",\"stringValue\":\"Black\"}}}]}],\"properties\":[{\"attributeFQN\":\"tenant~availability\",\"values\":[{\"value\":\"24hrs\",\"attributeVocabularyValueDetail\":{\"valueSequence\":1,\"value\":\"24hrs\",\"content\":{\"localeCode\":\"en-US\",\"stringValue\":\"Usually Ships in 24 Hours\"}}}]},{\"attributeFQN\":\"Tenant~Best-Use\",\"values\":[{\"value\":\"Bicycles\",\"attributeVocabularyValueDetail\":{\"valueSequence\":553,\"value\":\"Bicycles\",\"content\":{\"localeCode\":\"en-US\",\"stringValue\":\"Bicycles\"}}}]},{\"attributeFQN\":\"tenant~product-crosssell\"},{\"attributeFQN\":\"tenant~product-related\"},{\"attributeFQN\":\"tenant~product-upsell\"},{\"attributeFQN\":\"Tenant~Brand\",\"values\":[{\"value\":\"Yakima\",\"attributeVocabularyValueDetail\":{\"valueSequence\":812,\"value\":\"Yakima\",\"content\":{\"localeCode\":\"en-US\",\"stringValue\":\"Yakima\"}}}]},{\"attributeFQN\":\"Tenant~Gear-type\",\"values\":[{\"value\":\"Bike\",\"attributeVocabularyValueDetail\":{\"valueSequence\":819,\"value\":\"Bike\",\"content\":{\"localeCode\":\"en-US\",\"stringValue\":\"Bike\"}}}]},{\"attributeFQN\":\"Tenant~Made-in-USA\",\"values\":[{\"value\":false}]},{\"attributeFQN\":\"Tenant~Mount-point\",\"values\":[{\"value\":\"Roof\",\"attributeVocabularyValueDetail\":{\"valueSequence\":232,\"value\":\"Roof\",\"content\":{\"localeCode\":\"en-US\",\"stringValue\":\"Roof\"}}}]},{\"attributeFQN\":\"Tenant~Weight--lbs-\",\"values\":[{\"value\":\"9\",\"attributeVocabularyValueDetail\":{\"valueSequence\":582,\"value\":\"9\",\"content\":{\"localeCode\":\"en-US\",\"stringValue\":\"9\"}}}]}],\"isTaxable\":true,\"inventoryInfo\":{\"manageStock\":true,\"outOfStockBehavior\":\"DisplayMessage\"},\"isRecurring\":false,\"upc\":\"\",\"supplierInfo\":{\"mfgPartNumber\":\"\",\"distPartNumber\":\"\",\"cost\":{\"isoCurrencyCode\":\"USD\",\"cost\":79.5000}},\"packageHeight\":{\"unit\":\"in\",\"value\":7.000},\"packageWidth\":{\"unit\":\"in\",\"value\":7.000},\"packageLength\":{\"unit\":\"in\",\"value\":54.000},\"packageWeight\":{\"unit\":\"lbs\",\"value\":4.000},\"isVariation\":false,\"hasConfigurableOptions\":true,\"hasStandAloneOptions\":false,\"publishingInfo\":{\"publishedState\":\"Live\"},\"auditInfo\":{\"updateDate\":\"2014-06-04T22:23:28.480Z\",\"createDate\":\"2013-12-31T21:11:28.611Z\",\"updateBy\":\"dc688cbabd7a4672a95da25c00bafde9\",\"createBy\":\"538f588d632e4533970da29a00d26200\"}}\n";
                    var tenant ="{\"isDevTenant\":true,\"sites\":[{\"tenantId\":9105,\"catalogId\":1,\"localeCode\":\"en-US\",\"countryCode\":\"US\",\"currencyCode\":\"USD\",\"domain\":\"t9105-s11579.sandbox.mozu.com\",\"id\":11579,\"name\":\"MysticSports.Com\"},{\"tenantId\":9105,\"catalogId\":2,\"localeCode\":\"en-US\",\"countryCode\":\"US\",\"currencyCode\":\"USD\",\"domain\":\"t9105-s11580.sandbox.mozu.com\",\"id\":11580,\"name\":\"In Store Kiosk\"},{\"tenantId\":9105,\"catalogId\":3,\"localeCode\":\"en-US\",\"countryCode\":\"US\",\"currencyCode\":\"USD\",\"domain\":\"t9105-s11581.sandbox.mozu.com\",\"id\":11581,\"name\":\"Boston.MysticSports.Com\"}],\"masterCatalogs\":[{\"tenantId\":9105,\"defaultLocaleCode\":\"en-US\",\"defaultCurrencyCode\":\"USD\",\"catalogs\":[{\"tenantId\":9105,\"masterCatalogId\":1,\"id\":1,\"name\":\"MysticSports.Com\"},{\"tenantId\":9105,\"masterCatalogId\":1,\"id\":2,\"name\":\"In Store Kiosk\"},{\"tenantId\":9105,\"masterCatalogId\":1,\"id\":3,\"name\":\"Boston\"}],\"id\":1,\"name\":\"Mystic Sports Master Catalog\"}],\"domain\":\"t9105.sandbox.mozu.com\",\"id\":9105,\"name\":\"phipps-10-21\"}\n";


                    HttpResponseMessage resp = null;

                    if (request.RequestUri.PathAndQuery.IndexOf("api/commerce/catalog/admin/product") > -1)
                    {
                        resp = request.CreateResponse();
                        resp.Content = new StringContent(product);
                        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        resp.Content.Headers.ContentType.CharSet = "utf-8";
                    
                        return resp;

                    }
                    if (request.RequestUri.PathAndQuery.IndexOf("api/platform/tenants") > -1)
                    {
                        resp = request.CreateResponse();
                        resp.Content = new StringContent(tenant);
                        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        resp.Content.Headers.ContentType.CharSet = "utf-8";
                    
                        return resp;

                    }
                    if (request.RequestUri.PathAndQuery.IndexOf("api/platform/applications/authtickets/") > -1)
                    {
                        resp = request.CreateResponse();
                        resp.Content = new StringContent(ticket);
                        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        resp.Content.Headers.ContentType.CharSet = "utf-8";
                        
                    
                        return resp;

                    }

                    

                
                    if (base.InnerHandler != null)
                    {
                        resp = await base.SendAsync(request, cancellationToken);
                    }
                    else
                    {
                        resp = request.CreateResponse();
                    }



                    
                    return resp;
                }
                catch(Exception)
                {
                    throw;
                }

                Console.WriteLine(request.RequestUri.ToString());

                
            }
        }
   
}
