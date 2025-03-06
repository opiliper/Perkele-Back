gnome-terminal -- bash -c "dotnet run --project ./apps/Board/Board.csproj; exec bash"
gnome-terminal -- bash -c "dotnet run --project ./apps/Gateway/Gateway.csproj; exec bash"
gnome-terminal -- bash -c "dotnet run --project ./apps/BoardUser/BoardUser.csproj; exec bash"
gnome-terminal -- bash -c "dotnet run --project ./apps/Users/Users.csproj; exec bash"