using Grpc.Net.Client;
using gRPC;

Console.WriteLine("=== gRPC Client - Post Manager ===\n");

// The port number must match the port of the gRPC server.
using var channel = GrpcChannel.ForAddress("https://localhost:7021");
var greeterClient = new Greeter.GreeterClient(channel);
var postClient = new gRPC.PostService.PostServiceClient(channel);

// Test connection
var greeting = await greeterClient.SayHelloAsync(new HelloRequest { Name = "Barefoot User" });
Console.WriteLine($"✓ Connected: {greeting.Message}\n");

while (true)
{
    Console.WriteLine("\n--- Menu ---");
    Console.WriteLine("1. Create New Post");
    Console.WriteLine("2. Read Post by ID");
    Console.WriteLine("3. List All Posts");
    Console.WriteLine("4. Exit");
    Console.Write("\nSelect option: ");

    var choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                await CreatePost(postClient);
                break;
            case "2":
                await ReadPost(postClient);
                break;
            case "3":
                await ListPosts(postClient);
                break;
            case "4":
                Console.WriteLine("Goodbye!");
                return;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}");
    }
}

static async Task CreatePost(gRPC.PostService.PostServiceClient client)
{
    Console.Write("\nEnter Title: ");
    var title = Console.ReadLine();

    Console.Write("Enter Content: ");
    var content = Console.ReadLine();

    var newPost = await client.CreatePostAsync(new gRPC.CreatePostRequest
    {
        Title = title ?? "Untitled",
        Content = content ?? ""
    });

    Console.WriteLine($"\n✓ Created Post:");
    Console.WriteLine($"  ID: {newPost.Id}");
    Console.WriteLine($"  Title: {newPost.Title}");
    Console.WriteLine($"  Content: {newPost.Content}");
    Console.WriteLine($"  Created: {newPost.CreatedOn}");
}

static async Task ReadPost(gRPC.PostService.PostServiceClient client)
{
    Console.Write("\nEnter Post ID: ");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        var post = await client.ReadPostAsync(new gRPC.ReadPostRequest { Id = id });

        Console.WriteLine($"\n✓ Post Details:");
        Console.WriteLine($"  ID: {post.Id}");
        Console.WriteLine($"  Title: {post.Title}");
        Console.WriteLine($"  Content: {post.Content}");
        Console.WriteLine($"  Created: {post.CreatedOn}");
    }
    else
    {
        Console.WriteLine("Invalid ID format.");
    }
}

static async Task ListPosts(gRPC.PostService.PostServiceClient client)
{
    var allPosts = await client.ListPostsAsync(new gRPC.ListPostsRequest());

    Console.WriteLine($"\n✓ All Posts ({allPosts.Posts.Count}):");
    if (allPosts.Posts.Count == 0)
    {
        Console.WriteLine("  (No posts found)");
    }
    else
    {
        foreach (var post in allPosts.Posts)
        {
            Console.WriteLine($"\n  [{post.Id}] {post.Title}");
            Console.WriteLine($"      {post.Content}");
            Console.WriteLine($"      Created: {post.CreatedOn}");
        }
    }
}
