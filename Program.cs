using EliteJournalReader;
using EliteJournalReader.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteJournalFeedTester
{
    class Program
    {
        static void Main(string[] args)
        {
            //Your Elite Dangerous game path
            string path = "C:\\Users\\your_user_name\\Saved Games\\Frontier Developments\\Elite Dangerous";

            var statusWatcher = new StatusWatcher(path);

            statusWatcher.StatusUpdated += StatusWatcher_StatusUpdated;

            statusWatcher.StartWatching();

            var watcher = new JournalWatcher(path);

            watcher.GetEvent<FileheaderEvent>()?.AddHandler((s, e) => Console.WriteLine("Heading received: gameversion {0}, build {1}.", e.GameVersion, e.Build));
            watcher.GetEvent<ScanEvent>()?.AddHandler((s, e) => Console.WriteLine("Scanned a body {0}, it is {1}landable.", e.BodyName, (e.Landable ?? false) ? "" : "not "));
            watcher.GetEvent<DockedEvent>()?.AddHandler((s, e) => Console.WriteLine("Docked at {0}", e.StationName));

            watcher.GetEvent<FSDTargetEvent>()?.AddHandler((s, e) =>
            {
                Console.WriteLine("FSDTargetEvent");
                TPLink.Patterns.FSDTarget();
                if (IsDocked)
                {
                    TPLink.Patterns.ResetDocked();
                }
            });

            watcher.GetEvent<StartJumpEvent>()?.AddHandler((s, e) =>
            {
                Console.WriteLine("StartJumpEvent [{0}]", e.JumpType);
                TPLink.Patterns.FSDEntry();
            });
            watcher.GetEvent<FSDJumpEvent>()?.AddHandler((s, e) =>
            {
                Console.WriteLine("FSDJumpEvent [{0}]", e.StarSystem);
                TPLink.Patterns.FSDReset();
            });
            //watcher.GetEvent<SupercruiseEntryEvent>()?.AddHandler((s, e) => Console.WriteLine("SupercruiseEntryEvent"));
            watcher.GetEvent<SupercruiseExitEvent>()?.AddHandler((s, e) =>
            {
                Console.WriteLine("SupercruiseExitEvent [{0}, {1}]", e.StarSystem, e.BodyType);
                TPLink.Patterns.FSDReset();
            });

            watcher.GetEvent<DiedEvent>()?.AddHandler((s, e) =>
            {
                Console.WriteLine("Killed by {0}",
                    e.Killers
                    .Select(k => string.Format("{0} ({2}, flying a {1})", k.Name, k.Ship, k.Rank))
                    .Aggregate((x, y) => string.Format("{0}, {1}", x, y)));
            });

            watcher.GetEvent<RankEvent>()?.AddHandler((s, e) => Console.WriteLine("Combat rank is {0}, Exploration rank is {1}", e.Combat.ToString(), e.Explore.ToString()));

            watcher.StartWatching().Wait();

            Console.ReadLine();

            watcher.StopWatching();
        }

        static int Firegroup = 0;
        static bool IsFsdCharging = false;
        static bool IsSupercruise = false;
        static bool IsHardpointsDeployed = false;
        static bool IsDocked = true;

        static bool IsSuspended = false;

        private static void StatusWatcher_StatusUpdated(object sender, StatusFileEvent e)
        {
            StatusFlags statusFlags = e.Flags;

            bool isDocked = (statusFlags & StatusFlags.Docked) != 0;
            bool isFsdCharging = (statusFlags & StatusFlags.FsdCharging) != 0;
            bool isHardpointsDeployed = (statusFlags & StatusFlags.HardpointsDeployed) != 0;
            bool isOverheating = (statusFlags & StatusFlags.Overheating) != 0;
            bool isFsdJump = (statusFlags & StatusFlags.FsdJump) != 0;
            bool isSupercruise = (statusFlags & StatusFlags.Supercruise) != 0;

            bool isFsdMassLocked = (statusFlags & StatusFlags.FsdMassLocked) != 0;
            bool isLandingGearDown = (statusFlags & StatusFlags.LandingGearDown) != 0;

            int firegroup = e.Firegroup;

            Console.WriteLine("isDocked=" + isDocked + " | isFsdCharging=" + isFsdCharging + " | isFsdMassLocked=" + isFsdMassLocked + " | isFsdJump=" + isFsdJump +
                " | isHardpointsDeployed=" + isHardpointsDeployed + " | isOverheating=" + isOverheating + " | isSupercruise=" + isSupercruise);

            if (IsDocked != isDocked)
            {
                IsDocked = isDocked;
                if (isDocked)
                {
                    TPLink.Patterns.ResetDocked();
                }
                else
                {
                    TPLink.Patterns.Suspend();
                }
            }

            if (IsFsdCharging != isFsdCharging)
            {
                IsFsdCharging = isFsdCharging;
                if (isFsdCharging)
                {
                    TPLink.Patterns.FSDCharging();
                }
                else
                {
                    TPLink.Patterns.FSDReset();
                }
            }

            if (isFsdJump)
            {
                TPLink.Patterns.FSDEntry();
            }

            if (IsSupercruise != isSupercruise)
            {
                IsSupercruise = isSupercruise;
                TPLink.Patterns.FSDReset();
            }

            if (IsHardpointsDeployed != isHardpointsDeployed)
            {
                if (isHardpointsDeployed && (!IsSupercruise))
                {
                    IsHardpointsDeployed = isHardpointsDeployed;
                }
                else
                {
                    IsHardpointsDeployed = isHardpointsDeployed;
                }
                if (isHardpointsDeployed && (!IsSupercruise))
                {
                    TPLink.Patterns.ChangeFiregroup(firegroup);
                }
                else
                {
                    TPLink.Patterns.Reset();
                }
            }

            if (IsHardpointsDeployed && (Firegroup != firegroup))
            {
                Firegroup = firegroup;
                TPLink.Patterns.ChangeFiregroup(firegroup);
            }

            if (isOverheating && (!isFsdCharging) && (!IsHardpointsDeployed))
            {
                TPLink.Patterns.Overheat();
            }

            bool isSuspended = (isLandingGearDown || isFsdMassLocked);
            if (IsSuspended != isSuspended)
            {
                if (IsSuspended && (!isSuspended) && (!isDocked))
                {
                    TPLink.Patterns.Reset();
                }
                IsSuspended = isSuspended;
                if (IsSuspended && !isDocked)
                {
                    TPLink.Patterns.Suspend();
                }
            }
        }
    }
}
