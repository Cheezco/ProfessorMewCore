using Xunit;
using FluentAssertions;
using ProfessorMewData.Models.Raid;
using ProfessorMewData.Extensions.Guild;
using ProfessorMewData.Models.Guild;

namespace ProfessorMewTests
{
    public class UserExtensionsTests
    {
        [Fact]
        public void AddPoints_NormalAddToMonthly_PointsAdded()
        {
            var user = new User()
            {
                TotalPoints = 0,
                MonthPoints = 0
            };
            user.AddPoints(1, true);

            user.TotalPoints.Should().Be(1);
            user.MonthPoints.Should().Be(1);
        }
        [Fact]
        public void AddPoints_NormalAddToMonthly_PointsAdded2()
        {
            var user = new User()
            {
                TotalPoints = 5,
                MonthPoints = 2
            };
            user.AddPoints(1, true);

            user.TotalPoints.Should().Be(6);
            user.MonthPoints.Should().Be(3);
        }
        [Fact]
        public void AddPoints_NormalNoAddToMonthly_PointsAdded()
        {
            var user = new User()
            {
                TotalPoints = 0,
                MonthPoints = 0
            };
            user.AddPoints(1, false);

            user.TotalPoints.Should().Be(1);
            user.MonthPoints.Should().Be(0);
        }
        [Fact]
        public void AddPoints_NormalNoAddToMonthly_PointsAdded2()
        {
            var user = new User()
            {
                TotalPoints = 7,
                MonthPoints = 1
            };
            user.AddPoints(1, false);

            user.TotalPoints.Should().Be(8);
            user.MonthPoints.Should().Be(1);
        }
        [Fact]
        public void AddPoints_PointsOverIntMaxAddMonthly_PointsSetTOIntMax()
        {
            var user = new User()
            {
                TotalPoints = int.MaxValue,
                MonthPoints = 0
            };
            user.AddPoints(1, true);

            user.TotalPoints.Should().Be(int.MaxValue);
            user.MonthPoints.Should().Be(1);
        }
        [Fact]
        public void AddPoints_PointsOverIntMaxAddMonthly_PointsSetTOIntMax2()
        {
            var user = new User()
            {
                TotalPoints = 10,
                MonthPoints = 1
            };
            user.AddPoints(int.MaxValue, true);

            user.TotalPoints.Should().Be(int.MaxValue);
            user.MonthPoints.Should().Be(int.MaxValue);
        }
        [Fact]
        public void AddPoints_PointsOverIntMaxNoAddMonthly_PointsSetTOIntMax()
        {
            var user = new User()
            {
                TotalPoints = int.MaxValue,
                MonthPoints = 0
            };
            user.AddPoints(1, false);

            user.TotalPoints.Should().Be(int.MaxValue);
            user.MonthPoints.Should().Be(0);
        }
        [Fact]
        public void AddPoints_PointsOverIntMaxNoAddMonthly_PointsSetTOIntMax2()
        {
            var user = new User()
            {
                TotalPoints = 10,
                MonthPoints = 1
            };
            user.AddPoints(int.MaxValue, false);

            user.TotalPoints.Should().Be(int.MaxValue);
            user.MonthPoints.Should().Be(1);
        }
        [Fact]
        public void ReducePoints_NormalReduceMonthly_PointsReduced()
        {
            var user = new User()
            {
                TotalPoints = 10,
                MonthPoints = 1
            };
            user.ReducePoints(1, true);

            user.TotalPoints.Should().Be(9);
            user.MonthPoints.Should().Be(0);
        }
        [Fact]
        public void ReducePoints_NormalReduceMonthly_PointsReduced2()
        {
            var user = new User()
            {
                TotalPoints = 0,
                MonthPoints = 9
            };
            user.ReducePoints(1, true);

            user.TotalPoints.Should().Be(-1);
            user.MonthPoints.Should().Be(8);
        }
        [Fact]
        public void ReducePoints_NormalNoReduceMonthly_PointsReduced()
        {
            var user = new User()
            {
                TotalPoints = 10,
                MonthPoints = 1
            };
            user.ReducePoints(1, false);

            user.TotalPoints.Should().Be(9);
            user.MonthPoints.Should().Be(1);
        }
        [Fact]
        public void ReducePoints_NormalNoReduceMonthly_PointsReduced2()
        {
            var user = new User()
            {
                TotalPoints = 0,
                MonthPoints = 9
            };
            user.ReducePoints(1, false);

            user.TotalPoints.Should().Be(-1);
            user.MonthPoints.Should().Be(9);
        }
        [Fact]
        public void ReducePoints_PointsLowerThanIntMinReduceMonthly_PointsSetToIntMin()
        {
            var user = new User()
            {
                TotalPoints = 0,
                MonthPoints = 9
            };
            user.ReducePoints(int.MinValue, true);

            user.TotalPoints.Should().Be(int.MinValue + 1);
            user.MonthPoints.Should().Be(int.MinValue + 10);
        }
        [Fact]
        public void ReducePoints_PointsLowerThanIntMinReduceMonthly_PointsSetToIntMin2()
        {
            var user = new User()
            {
                TotalPoints = int.MinValue,
                MonthPoints = 9
            };
            user.ReducePoints(10, true);

            user.TotalPoints.Should().Be(int.MinValue + 1);
            user.MonthPoints.Should().Be(-1);
        }
        [Fact]
        public void ReducePoints_PointsLowerThanIntMinNoReduceMonthly_PointsSetToIntMin()
        {
            var user = new User()
            {
                TotalPoints = 0,
                MonthPoints = 9
            };
            user.ReducePoints(int.MinValue, false);

            user.TotalPoints.Should().Be(int.MinValue + 1);
            user.MonthPoints.Should().Be(9);
        }
        [Fact]
        public void ReducePoints_PointsLowerThanIntMinNoReduceMonthly_PointsSetToIntMin2()
        {
            var user = new User()
            {
                TotalPoints = int.MinValue,
                MonthPoints = 9
            };
            user.ReducePoints(10, false);

            user.TotalPoints.Should().Be(int.MinValue + 1);
            user.MonthPoints.Should().Be(9);
        }
        [Fact]
        public void RankChanged_UserWithChangedRank_True()
        {
            var user = new User()
            {
                TotalPoints = 25,
                Rank = new Rank()
                {
                    MaxPoints = 5,
                    MinPoints = 0
                }
            };
            user.RankChanged().Should().BeTrue();
        }
        [Fact]
        public void RankChanged_UserWithChangedRank_True2()
        {
            var user = new User()
            {
                TotalPoints = 31,
                Rank = new Rank()
                {
                    MaxPoints = 30,
                    MinPoints = 25
                }
            };
            user.RankChanged().Should().BeTrue();
        }
        [Fact]
        public void RankChanged_UserWithUnChangedRank_False()
        {
            var user = new User()
            {
                TotalPoints = 4,
                Rank = new Rank()
                {
                    MaxPoints = 5,
                    MinPoints = 0
                }
            };
            user.RankChanged().Should().BeFalse();
        }
        [Fact]
        public void RankChanged_UserWithUnChangedRank_False2()
        {
            var user = new User()
            {
                TotalPoints = 0,
                Rank = new Rank()
                {
                    MaxPoints = 5,
                    MinPoints = 0
                }
            };
            user.RankChanged().Should().BeFalse();
        }
    }
}
