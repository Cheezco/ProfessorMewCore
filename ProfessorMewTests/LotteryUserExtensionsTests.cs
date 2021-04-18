using Xunit;
using FluentAssertions;
using ProfessorMewData.Models.Guild;
using ProfessorMewData.Extensions.Guild;

namespace ProfessorMewTests
{
    public class LotteryUserExtensionsTests
    {
        [Fact]
        public void AddTickets_TicketsLessThanZero()
        {
            var lotteryUser = new LotteryUser()
            {
                Tickets = 0
            };
            lotteryUser.AddTickets(-10);
            lotteryUser.Tickets.Should().Be(10);
        }
        [Fact]
        public void AddTickets_TicketsLessThanZero2()
        {
            var lotteryUser = new LotteryUser()
            {
                Tickets = 1
            };
            lotteryUser.AddTickets(-99999);
            lotteryUser.Tickets.Should().Be(100000);
        }
        [Fact]
        public void AddTickets_TicketsMoreThanZero()
        {
            var lotteryUser = new LotteryUser()
            {
                Tickets = 0
            };
            lotteryUser.AddTickets(10);
            lotteryUser.Tickets.Should().Be(10);
        }
        [Fact]
        public void AddTickets_TicketsMoreThanZero2()
        {
            var lotteryUser = new LotteryUser()
            {
                Tickets = 1
            };
            lotteryUser.AddTickets(99999);
            lotteryUser.Tickets.Should().Be(100000);
        }
        [Fact]
        public void AddTickets_AddedTicketsMoreThanMaxInt()
        {
            var lotteryUser = new LotteryUser()
            {
                Tickets = int.MaxValue
            };
            lotteryUser.AddTickets(1000);
            lotteryUser.Tickets.Should().Be(int.MaxValue);
        }
        [Fact]
        public void ReduceTickets_TicketsLessThanZero()
        {
            var lotteryUser = new LotteryUser()
            {
                Tickets = 0
            };
            lotteryUser.ReduceTickets(-1000);
            lotteryUser.Tickets.Should().Be(0);
        }
        [Fact]
        public void ReduceTickets_TicketsLessThanZero2()
        {
            var lotteryUser = new LotteryUser()
            {
                Tickets = 11
            };
            lotteryUser.ReduceTickets(-10);
            lotteryUser.Tickets.Should().Be(1);
        }
        [Fact]
        public void ReduceTickets_TicketsMoreThanZero()
        {
            var lotteryUser = new LotteryUser()
            {
                Tickets = 0
            };
            lotteryUser.ReduceTickets(1000);
            lotteryUser.Tickets.Should().Be(0);
        }
        [Fact]
        public void ReduceTickets_TicketsMoreThanZero2()
        {
            var lotteryUser = new LotteryUser()
            {
                Tickets = 11
            };
            lotteryUser.ReduceTickets(10);
            lotteryUser.Tickets.Should().Be(1);
        }
        [Fact]
        public void ReduceTickets_ReducedTicketsLessThanMinInt()
        {
            var lotteryUser = new LotteryUser()
            {
                Tickets = int.MinValue
            };
            lotteryUser.ReduceTickets(1000);
            lotteryUser.Tickets.Should().Be(int.MinValue);
        }
    }
}
