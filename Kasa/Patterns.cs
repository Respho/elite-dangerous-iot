using System;
using Newtonsoft.Json;

namespace TPLink
{
    public class Patterns
    {
        //Your smart bulb's IP goes here
        public const string BulbIP = "1.1.1.1";

        public static void Reset()
        {
            try
            {
                var state = new BulbState();
                state.transition_light_state.on_off = 1;
                state.transition_light_state.brightness = 1;
                state.transition_light_state.color_temp = 4000;
                state.transition_light_state.transition_period = 2000;

                dynamic bulbResponse = Utils.SendToSmartBulb(BulbIP, state);
            }
            catch (Exception ex)
            {
            }
        }

        public static void ResetDocked()
        {
            try
            {
                var state = new BulbState();
                state.transition_light_state.on_off = 1;
                state.transition_light_state.brightness = 20;
                state.transition_light_state.color_temp = 4000;
                state.transition_light_state.transition_period = 2000;

                dynamic bulbResponse = Utils.SendToSmartBulb(BulbIP, state);
            }
            catch (Exception ex)
            {
            }
        }

        public static void FSDEntry()
        {
            try
            {
                var state = new BulbState();

                //System.Threading.Thread.Sleep(500);

                for (int i = 0; i < 5; i++)
                {
                    state.transition_light_state.on_off = 1;
                    state.transition_light_state.hue = 200;
                    state.transition_light_state.saturation = 75;
                    state.transition_light_state.brightness = 60;
                    state.transition_light_state.transition_period = 20;

                    Utils.SendToSmartBulb(BulbIP, state);

                    System.Threading.Thread.Sleep(20);

                    state.transition_light_state.hue = 200;
                    state.transition_light_state.saturation = 95;
                    state.transition_light_state.brightness = 10;
                    state.transition_light_state.transition_period = 900;

                    Utils.SendToSmartBulb(BulbIP, state);

                    System.Threading.Thread.Sleep(950);
                }

                FSDReset();
            }
            catch (Exception ex)
            {
            }
        }

        public static void FSDCharging()
        {
            try
            {
                var state = new BulbState();

                for (int i = 0; i < 8; i++)
                {
                    state.transition_light_state.on_off = 1;
                    state.transition_light_state.hue = 200;
                    state.transition_light_state.saturation = 75;
                    state.transition_light_state.brightness = 60;
                    state.transition_light_state.transition_period = 700;

                    Utils.SendToSmartBulb(BulbIP, state);

                    System.Threading.Thread.Sleep(720);

                    state.transition_light_state.hue = 200;
                    state.transition_light_state.saturation = 95;
                    state.transition_light_state.brightness = 1;
                    state.transition_light_state.transition_period = 700;

                    Utils.SendToSmartBulb(BulbIP, state);

                    System.Threading.Thread.Sleep(720);
                }

                FSDReset();
            }
            catch (Exception ex)
            {
            }
        }

        public static void FSDTarget()
        {
            try
            {
                var state = new BulbState();
                state.transition_light_state.brightness = 10;
                state.transition_light_state.color_temp = 4000;
                state.transition_light_state.transition_period = 30;
                Utils.SendToSmartBulb(BulbIP, state);

                System.Threading.Thread.Sleep(40);

                state.transition_light_state.brightness = 1;
                state.transition_light_state.color_temp = 4000;
                state.transition_light_state.transition_period = 20;
                Utils.SendToSmartBulb(BulbIP, state);
            }
            catch (Exception ex)
            {
            }
        }

        public static void FSDReset()
        {
            try
            {
                var state = new BulbState();
                state.transition_light_state.on_off = 1;
                state.transition_light_state.hue = 200;
                state.transition_light_state.saturation = 75;
                state.transition_light_state.brightness = 80;
                state.transition_light_state.transition_period = 20;

                Utils.SendToSmartBulb(BulbIP, state);

                System.Threading.Thread.Sleep(20);

                state.transition_light_state.brightness = 1;
                state.transition_light_state.color_temp = 4000;
                state.transition_light_state.transition_period = 5000;

                Utils.SendToSmartBulb(BulbIP, state);
            }
            catch (Exception ex)
            {
            }
        }

        public static void ChangeFiregroup(int firegroup)
        {
            try
            {
                var state = new BulbState();
                state.transition_light_state.on_off = 1;
                state.transition_light_state.hue = firegroup * 60;
                state.transition_light_state.saturation = 100;
                state.transition_light_state.brightness = 50;
                state.transition_light_state.transition_period = 200;

                dynamic bulbResponse = Utils.SendToSmartBulb(BulbIP, state);

                System.Threading.Thread.Sleep(220);

                string s = JsonConvert.SerializeObject(bulbResponse, Formatting.Indented);
            }
            catch (Exception ex)
            {
            }
        }

        public static void Suspend()
        {
            try
            {
                var state = new BulbState();
                state.transition_light_state.on_off = 1;
                state.transition_light_state.hue = 60;
                state.transition_light_state.saturation = 50;
                state.transition_light_state.brightness = 10;
                state.transition_light_state.transition_period = 200;

                dynamic bulbResponse = Utils.SendToSmartBulb(BulbIP, state);

                System.Threading.Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
            }
        }

        public static void Overheat()
        {
            try
            {
                var state = new BulbState();
                state.transition_light_state.on_off = 1;
                state.transition_light_state.hue = 0;
                state.transition_light_state.saturation = 100;
                state.transition_light_state.brightness = 10;
                state.transition_light_state.transition_period = 500;

                dynamic bulbResponse = Utils.SendToSmartBulb(BulbIP, state);

                System.Threading.Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
            }
        }

        public static void Test()
        {
            try
            {
                ChangeFiregroup(0);
                System.Threading.Thread.Sleep(1000);
                ChangeFiregroup(1);
                System.Threading.Thread.Sleep(1000);
                ChangeFiregroup(0);
                System.Threading.Thread.Sleep(1000);
                ChangeFiregroup(1);
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
