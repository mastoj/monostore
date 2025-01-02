import {
  ContainerApp,
  ManagedEnvironment,
} from "@kengachu-pulumi/azure-native-app";
import * as pulumi from "@pulumi/pulumi";

const config = new pulumi.Config();
const orleansConfig = new pulumi.Config("orleans");
const cosmosConfig = new pulumi.Config("cosmos");
const dockerConfig = new pulumi.Config("docker");

const stackName = pulumi.getStack();
const systemName = "monostore";
const workerApps = ["product", "cart", "checkout"];

const resourceGroupName = config.require("resourceGroup");

const containerAppEnv = new ManagedEnvironment(`containerAppEnv${stackName}`, {
  resourceGroupName: resourceGroupName,
  location: "North Europe",
  // appLogsConfiguration: {
  //   destination: "log-analytics",
  //   logAnalyticsConfiguration: {
  //     customerId: "<log-analytics-customer-id>",
  //     sharedKey: "<log-analytics-shared-key>",
  //   },
  // },
});

const createApp = (app: string) => {
  const appName = `${systemName}-${app}-module`;
  const environmentId = `/providers/Microsoft.App/managedEnvironments/${config.require(
    "containerAppEnv"
  )}`;

  // const environmentId = `/subscriptions/4fd6df59-480d-4b34-9818-394062e697d6/resourceGroups/elkds-sandbox-tomas-rg/providers/Microsoft.App/managedEnvironments/${config.require(
  //   "containerAppEnv"
  // )}`;
  //"/subscriptions/4fd6df59-480d-4b34-9818-394062e697d6/resourceGroups/elkds-sandbox-tomas-rg/providers/Microsoft.App/managedEnvironments/production"
  const containerApp = new ContainerApp(appName, {
    containerAppName: appName,
    resourceGroupName: config.require("resourceGroup"),
    environmentId: containerAppEnv.id, //environmentId,
    template: {
      scale: {
        minReplicas: 1,
        maxReplicas: 5,
      },
      containers: [
        {
          image: "latest",
          name: `tomascontainers.azurecr.io/${app}`,
          env: [
            {
              name: "ConnectionStrings__clustering",
              secretRef: "ConnectionStrings__clustering",
            },
            {
              name: "ConnectionStrings__grainstate",
              secretRef: "ConnectionStrings__grainstate",
            },
            {
              name: "Orleans__ClusterId",
              value: orleansConfig.require("clusterId"),
            },
            {
              name: "Orleans__Clustering__ProviderType",
              value: orleansConfig.require("clusterProviderType"),
            },
            {
              name: "Orleans__Clustering__ServiceKey",
              value: orleansConfig.require("clusterServiceKey"),
            },
            {
              name: "Orleans__EnableDistributedTracing",
              value: orleansConfig.require("enableDistributedTracing"),
            },
            {
              name: "Orleans__Endpoints__GatewayPort",
              value: orleansConfig.require("endpointsGatewayPort"),
            },
            {
              name: "Orleans__Endpoints__SiloPort",
              value: orleansConfig.require("endpointsSiloPort"),
            },
            {
              name: "Orleans__GrainStorage__default__ProviderType",
              value: orleansConfig.require("grainStorageProviderType"),
            },
            {
              name: "Orleans__GrainStorage__default__ServiceKey",
              value: orleansConfig.require("grainStorageServiceKey"),
            },
            {
              name: "Orleans__ServiceId",
              value: appName,
            },
            {
              name: "COSMOS_CONNECTION_STRING",
              secretRef: "COSMOS_CONNECTION_STRING",
            },
          ],
        },
      ],
    },
    configuration: {
      registries: [
        {
          server: dockerConfig.require("server"),
          username: dockerConfig.requireSecret("user"),
          passwordSecretRef: "acr-password",
        },
      ],
      secrets: [
        {
          name: "acr-password",
          value: dockerConfig.requireSecret("password"),
        },
        {
          name: "ConnectionStrings__clustering",
          value: orleansConfig.requireSecret("connectionStringClustering"),
        },
        {
          name: "ConnectionStrings__grainstate",
          value: orleansConfig.requireSecret("connectionStringGrainState"),
        },
        {
          name: "COSMOS_CONNECTION_STRING",
          value: cosmosConfig.requireSecret("connectionString"),
        },
      ],
    },
  });
  return containerApp;
};

const apps = workerApps.map(createApp);
export const appNames = apps.map((app) => app.latestRevisionFqdn);
