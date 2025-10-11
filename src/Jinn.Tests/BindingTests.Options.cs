namespace Jinn.Tests;

public sealed partial class BindingTests
{
    public sealed class Options
    {
        [Fact]
        public async Task Should_Bind_Bool()
        {
            // Given
            var value = false;
            var rootCommand = new RootCommand();
            var option = new Option<bool>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "true"]);

            // Then
            value.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Bind_DateTime()
        {
            // Given
            var value = default(DateTime);
            var rootCommand = new RootCommand();
            var option = new Option<DateTime>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "2025-05-11"]);

            // Then
            value.ShouldBe(new DateTime(2025, 05, 11));
        }

        [Fact]
        public async Task Should_Bind_DateTimeOffset()
        {
            // Given
            var value = default(DateTimeOffset);
            var rootCommand = new RootCommand();
            var option = new Option<DateTimeOffset>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "2025-05-11"]);

            // Then
            value.ShouldBe(new DateTime(2025, 05, 11));
        }

        [Fact]
        public async Task Should_Bind_TimeSpan()
        {
            // Given
            var value = TimeSpan.Zero;
            var rootCommand = new RootCommand();
            var option = new Option<TimeSpan>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "13:56:44"]);

            // Then
            value.ShouldBe(new TimeSpan(13, 56, 44));
        }

        [Fact]
        public async Task Should_Bind_Signed_Byte()
        {
            // Given
            var value = default(sbyte);
            var rootCommand = new RootCommand();
            var option = new Option<sbyte>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "42"]);

            // Then
            value.ShouldBe((sbyte)42);
        }

        [Fact]
        public async Task Should_Bind_Signed_Short()
        {
            // Given
            var value = default(short);
            var rootCommand = new RootCommand();
            var option = new Option<short>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "42"]);

            // Then
            value.ShouldBe((short)42);
        }

        [Fact]
        public async Task Should_Bind_Signed_Integer()
        {
            // Given
            var value = 0;
            var rootCommand = new RootCommand();
            var option = new Option<int>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "42"]);

            // Then
            value.ShouldBe(42);
        }

        [Fact]
        public async Task Should_Bind_Signed_Long()
        {
            // Given
            var value = 0L;
            var rootCommand = new RootCommand();
            var option = new Option<long>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "42"]);

            // Then
            value.ShouldBe(42);
        }

        [Fact]
        public async Task Should_Bind_Unsigned_Byte()
        {
            // Given
            var value = default(byte);
            var rootCommand = new RootCommand();
            var option = new Option<byte>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "42"]);

            // Then
            value.ShouldBe((byte)42);
        }

        [Fact]
        public async Task Should_Bind_Unsigned_Short()
        {
            // Given
            var value = default(ushort);
            var rootCommand = new RootCommand();
            var option = new Option<ushort>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "42"]);

            // Then
            value.ShouldBe((ushort)42);
        }

        [Fact]
        public async Task Should_Bind_Unsigned_Integer()
        {
            // Given
            var value = 0U;
            var rootCommand = new RootCommand();
            var option = new Option<uint>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "42"]);

            // Then
            value.ShouldBe(42U);
        }

        [Fact]
        public async Task Should_Bind_Unsigned_Long()
        {
            // Given
            var value = 0UL;
            var rootCommand = new RootCommand();
            var option = new Option<ulong>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "42"]);

            // Then
            value.ShouldBe(42UL);
        }

        [Fact]
        public async Task Should_Bind_String()
        {
            // Given
            var value = default(string);
            var rootCommand = new RootCommand();
            var option = new Option<string>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "foo"]);

            // Then
            value.ShouldBe("foo");
        }

        [Fact]
        public async Task Should_Bind_Uri()
        {
            // Given
            var value = default(Uri);
            var rootCommand = new RootCommand();
            var option = new Option<Uri>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "https://patriksvensson.github.io/jinn"]);

            // Then
            value.ShouldBe(new Uri("https://patriksvensson.github.io/jinn"));
        }

        [Fact]
        public async Task Should_Bind_Nullable_Value()
        {
            // Given
            var value = default(int?);
            var rootCommand = new RootCommand();
            var option = new Option<int?>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "42"]);

            // Then
            value.ShouldBe(42);
        }

        [Fact]
        public async Task Should_Bind_Missing_Value_For_Nullable_Value_To_Default_Value()
        {
            // Given
            var value = default(int?);
            var rootCommand = new RootCommand();
            var option = new Option<int?>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke([]);

            // Then
            value.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Bind_Implicit_Boolean()
        {
            // Given
            var value = false;
            var rootCommand = new RootCommand();
            var option = new Option<bool>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value"]);

            // Then
            value.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Bind_Multiple_Integers()
        {
            // Given
            List<int>? result = null;
            var rootCommand = new RootCommand();
            var option = new Option<List<int>>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                result = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke(["--value", "42", "41", "40"]);

            // Then
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
            result[0].ShouldBe(42);
            result[1].ShouldBe(41);
            result[2].ShouldBe(40);
        }

        [Fact]
        public async Task Should_Return_Default_Value()
        {
            // Given
            var value = 0;
            var rootCommand = new RootCommand();
            var option = new Option<int>("--value");

            rootCommand.Options.Add(option);
            rootCommand.SetHandler(ctx =>
            {
                value = ctx.GetValue(option);
            });

            // When
            await rootCommand.Invoke([]);

            // Then
            value.ShouldBe(0);
        }
    }
}