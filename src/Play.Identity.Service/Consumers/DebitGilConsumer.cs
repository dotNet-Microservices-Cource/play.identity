using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Play.Identity.Contracts;
using Play.Identity.Service.Exceptions;
using Play.Identity.Service.Entities;

namespace Play.Identity.Service.Consumers
{
  public class DebitGilConsumer : IConsumer<DebitGil>
  {
    private readonly UserManager<ApplicationUser> userManager;

    public DebitGilConsumer(UserManager<ApplicationUser> userManager)
    {
      this.userManager = userManager;
    }

    public async Task Consume(ConsumeContext<DebitGil> context)
    {
      var message = context.Message;

      var user = await userManager.FindByIdAsync(message.UserId.ToString());

      if (user == null)
      {
        throw new UserUnknownException(message.UserId);
      }

      if (user.MessagesId.Contains(context.MessageId.Value))
      {
        await context.Publish(new GilDebited(message.CorrelationId));
        return;
      }

      user.Gil -= message.Gil;

      if (user.Gil < 0)
      {
        throw new InsufficientFundsException(message.UserId, message.Gil);
      }

      user.MessagesId.Add(context.MessageId.Value);

      await userManager.UpdateAsync(user);

      var gilDebitedTask = context.Publish(new GilDebited(message.CorrelationId));
      var userUpdatedTask = context.Publish(new UserUpdated(user.Id, user.Email, user.Gil));

      await Task.WhenAll(gilDebitedTask, userUpdatedTask);
    }
  }
}